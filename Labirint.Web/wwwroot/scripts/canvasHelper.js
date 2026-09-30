const commandTypes = {
    0: 'beginPath',
    1: 'moveTo',
    2: 'lineTo',
    3: 'stroke',
    4: 'drawImage',
    5: 'strokeStyle',
    6: 'lineWidth',
    7: 'clearRect',
    8: 'fillRect',
    9: 'drawSprites',
    10: 'fillStyle',
};

const spriteStride = 8;

const commandHandlers = {
    beginPath: context => context.beginPath(),
    moveTo: (context, command) => context.moveTo(command.x, command.y),
    lineTo: (context, command) => context.lineTo(command.x, command.y),
    stroke: context => context.stroke(),
    strokeStyle: (context, command) => {
        context.strokeStyle = command.color;
    },
    fillStyle: (context, command) => {
        context.fillStyle = command.color;
    },
    lineWidth: (context, command) => {
        context.lineWidth = command.width;
    },
    clearRect: (context, command) => context.clearRect(command.x, command.y, command.width, command.height),
    fillRect: (context, command) => context.fillRect(command.x, command.y, command.width, command.height),
    drawImage: (context, command) => {
        const image = imageCache[command.source];

        if (image) {
            context.drawImage(image, command.x, command.y, command.width, command.height);
        }
    },
    drawSprites: (context, command) => {
        const image = imageCache[command.source];

        if (image === undefined) {
            return;
        }

        const sprites = command.sprites;

        for (let offset = 0; offset < sprites.length; offset += spriteStride) {
            context.drawImage(image,
                sprites[offset], sprites[offset + 1], sprites[offset + 2], sprites[offset + 3],
                sprites[offset + 4], sprites[offset + 5], sprites[offset + 6], sprites[offset + 7]);
        }
    },
};

const imageCache = {};

const drawGenerations = new WeakMap();

const loadImage = source =>
    new Promise(resolve => {
        const image = new Image();
        image.src = source;
        image.onload = () => {
            imageCache[source] = image;
            resolve();
        };
        image.onerror = () => {
            console.warn(`Не удалось загрузить изображение: ${source}`);
            resolve();
        };
    });

const collectMissingSources = drawCommands => {
    const missing = new Set();

    for (const command of drawCommands) {
        if (command.source && imageCache[command.source] === undefined) {
            missing.add(command.source);
        }
    }

    return [...missing];
};

window.canvasHelper = {
    getContext2D(canvas) {
        return canvas.getContext('2d');
    },
    drawCommands(context, drawCommands) {
        const generation = (drawGenerations.get(context) ?? 0) + 1;
        drawGenerations.set(context, generation);

        const isCurrent = () => drawGenerations.get(context) === generation;

        async function draw() {
            const missing = collectMissingSources(drawCommands);

            if (missing.length > 0) {
                await Promise.all(missing.map(loadImage));

                if (isCurrent() === false) {
                    return;
                }
            }

            context.clearRect(0, 0, context.canvas.width, context.canvas.height);

            for (const command of drawCommands) {
                const handler = commandHandlers[commandTypes[command.type]];

                if (handler === undefined) {
                    console.warn(`Неизвестный тип команды: ${command.type}`);
                    continue;
                }

                handler(context, command);
            }
        }

        requestAnimationFrame(draw);
    }
};
