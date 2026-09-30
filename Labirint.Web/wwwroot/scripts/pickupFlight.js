window.labirintPickup = (() => {
    const findSlot = name => [...document.querySelectorAll('[data-item]')].find(slot => slot.dataset.item === name) ?? null;

    const isMotionReduced = () => window.labirintPage.isMotionReduced();

    const start = (name, icon, duration, isReturned, attempt) => {
        const runner = document.querySelector('[data-runner]');
        const slot = findSlot(name);

        if (runner === null || slot === null || slot.clientWidth === 0) {
            if (attempt < 3) {
                requestAnimationFrame(() => start(name, icon, duration, isReturned, attempt + 1));
            }

            return;
        }

        const source = isReturned ? slot : runner;
        const target = isReturned ? runner : slot;

        const from = source.getBoundingClientRect();
        const to = target.getBoundingClientRect();

        if (from.width === 0 || to.width === 0) {
            return;
        }

        const size = isReturned ? to.width : from.width;

        const ghost = document.createElement('img');

        ghost.src = icon;
        ghost.alt = '';
        ghost.setAttribute('aria-hidden', 'true');
        ghost.className = 'pickup-ghost';
        ghost.style.left = `${from.left + (from.width - size) / 2}px`;
        ghost.style.top = `${from.top + (from.height - size) / 2}px`;
        ghost.style.width = `${size}px`;
        ghost.style.height = `${size}px`;

        document.body.appendChild(ghost);

        const shiftX = to.left + (to.width - size) / 2 - from.left - (from.width - size) / 2;
        const shiftY = to.top + (to.height - size) / 2 - from.top - (from.height - size) / 2;
        const lift = Math.min(90, Math.max(24, Math.abs(shiftY) * 0.3));
        const startScale = isReturned ? to.width * 0.85 / size : 1;
        const endScale = isReturned ? 1 : to.width * 0.85 / size;

        const animation = ghost.animate([
            {
                offset: 0,
                transform: `translate(0, 0) scale(${startScale}) rotate(0deg)`,
                opacity: isReturned ? 0.6 : 0.9,
            },
            {
                offset: 0.35,
                transform: `translate(${shiftX * 0.4}px, ${shiftY * 0.25 - lift}px) scale(1.2) rotate(${isReturned ? 10 : -10}deg)`,
                opacity: 1,
            },
            {
                offset: 1,
                transform: `translate(${shiftX}px, ${shiftY}px) scale(${endScale}) rotate(0deg)`,
                opacity: isReturned ? 0 : 0.95,
            },
        ], {
            duration: duration,
            easing: 'cubic-bezier(0.35, 0, 0.2, 1)',
            fill: 'forwards',
        });

        animation.finished.catch(() => {}).finally(() => ghost.remove());
    };

    return {
        fly: (name, icon, duration) => {
            if (isMotionReduced()) {
                return;
            }

            start(name, icon, duration, false, 0);
        },
        cast: (name, icon, duration) => {
            if (isMotionReduced()) {
                return;
            }

            start(name, icon, duration, true, 0);
        },
    };
})();
