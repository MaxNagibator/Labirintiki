const commandTypes = {
    0: 'beginPath',
    1: 'moveTo',
    2: 'lineTo',
    3: 'stroke',
    4: 'drawImage'
};

window.canvasHelper = {
    getContext2D(canvas) {
        return canvas.getContext('2d');
    },
    setLineWidth(context, width) {
        context.lineWidth = width;
    },
    setStrokeStyle(context, color) {
        context.strokeStyle = color;
    },
    drawCommands(contextRef, drawCommands) {
        function draw() {
            for (const command of drawCommands) {
                if (commandTypes[command.type] === "beginPath") {
                    contextRef.beginPath();
                } else if (commandTypes[command.type] === "moveTo") {
                    contextRef.moveTo(command.x, command.y);
                } else if (commandTypes[command.type] === "lineTo") {
                    contextRef.lineTo(command.x, command.y);
                } else if (commandTypes[command.type] === "stroke") {
                    contextRef.stroke();
                } else if (commandTypes[command.type] === "drawImage") {
                    const img = new Image();
                    img.src = command.source;
                    img.onload = () => contextRef.drawImage(img, command.x, command.y, command.size, command.size);
                }
            }
        }

        requestAnimationFrame(draw);
    }
};
