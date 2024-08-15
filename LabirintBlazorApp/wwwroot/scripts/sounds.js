async function playSound(soundType) {
    let audio = new Audio();

    switch (soundType) {
        case "bomb":
            audio.src = "/media/baxbax.mp3";
            break;
        case "score":
            audio.src = "/media/score.mp3";
            break;
        case "step":
            var rand = Math.floor(Math.random() * 13) + 1;
            audio.src = "/media/steps/step" + rand + ".mp3";
            break;
        case "molot":
            var rand = Math.floor(Math.random() * 2) + 1;
            audio.src = "/media/molots/molot" + rand + ".mp3";
            break;
        default:
            audio.src = soundType;
            break;
    }

    await audio.play();
}
 