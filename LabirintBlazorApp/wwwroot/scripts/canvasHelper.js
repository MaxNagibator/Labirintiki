const commandTypes = {
    0: 'beginPath',
    1: 'moveTo',
    2: 'lineTo',
    3: 'stroke',
    4: 'drawImage',
    5: 'strokeStyle',
    6: 'lineWidth',
    7: 'clearRect',
    8: 'strokeRect',
    9: 'drawSprite',
};

const commandHandlers = {
    beginPath: context => context.beginPath(),
    moveTo: (context, command) => context.moveTo(command.x, command.y),
    lineTo: (context, command) => context.lineTo(command.x, command.y),
    stroke: context => context.stroke(),
    strokeStyle: (context, command) => {
        context.strokeStyle = command.color;
        context.fillStyle = command.color;
    },
    lineWidth: (context, command) => {
        context.lineWidth = command.width;
    },
    clearRect: (context, command) => context.clearRect(command.x, command.y, command.width, command.height),
    strokeRect: (context, command) => context.fillRect(command.x, command.y, command.width, command.height),
    drawImage: async (context, command) => {
        const img = await loadImage(command.source);
        context.drawImage(img, command.x, command.y, command.width, command.height);
    },
    drawSprite: async (context, command) => {
        const img = await loadImage(command.source);
        context.drawImage(img, command.sourceX, command.sourceY, command.sourceWidth, command.sourceHeight, command.x, command.y, command.width, command.height);
    }
};

const imageCache = {};

const loadImage = source =>
    new Promise((resolve, reject) => {
        if (imageCache[source]) {
            resolve(imageCache[source]);
        } else {
            const image = new Image();
            image.src = source;
            image.onload = () => {
                imageCache[source] = image;
                resolve(image);
            };
            image.onerror = reject;
        }
    });

window.canvasHelper = {
    getContext2D(canvas) {
        return canvas.getContext('2d');
    },
    drawCommands(context, drawCommands) {
        const offScreenCanvas = document.createElement('canvas');
        offScreenCanvas.width = context.canvas.width;
        offScreenCanvas.height = context.canvas.height;
        const offScreenContext = offScreenCanvas.getContext('2d');

        async function draw() {
            for (const command of drawCommands) {
                const handler = commandHandlers[commandTypes[command.type]];
                if (handler) {
                    await handler(offScreenContext, command);
                } else {
                    console.warn(`Неизвестный тип команды: ${command.type}`);
                }
            }

            context.clearRect(0, 0, context.canvas.width, context.canvas.height);
            context.drawImage(offScreenCanvas, 0, 0);
        }

        requestAnimationFrame(draw);
    }
};
