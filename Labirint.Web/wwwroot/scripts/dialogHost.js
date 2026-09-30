window.labirintDialog = (() => {
    const focusable = 'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

    const layers = [];

    const current = () => layers.length > 0 ? layers[layers.length - 1] : null;

    const items = element => [...element.querySelectorAll(focusable)].filter(item => item.offsetParent !== null);

    const focusInto = element => {
        requestAnimationFrame(() => {
            const layer = current();

            if (layer === null || layer.element !== element) {
                return;
            }

            const reachable = items(element);
            (reachable.length > 0 ? reachable[0] : element).focus();
        });
    };

    const onKeyDown = event => {
        const layer = current();

        if (event.key !== 'Tab' || layer === null) {
            return;
        }

        const reachable = items(layer.element);

        if (reachable.length === 0) {
            event.preventDefault();
            layer.element.focus();
            return;
        }

        const first = reachable[0];
        const last = reachable[reachable.length - 1];

        if (layer.element.contains(document.activeElement) === false) {
            event.preventDefault();
            (event.shiftKey ? last : first).focus();
            return;
        }

        if (event.shiftKey && document.activeElement === first) {
            event.preventDefault();
            last.focus();
        } else if (!event.shiftKey && document.activeElement === last) {
            event.preventDefault();
            first.focus();
        }
    };

    return {
        trap: (element, key) => {
            const index = layers.findIndex(layer => layer.key === key);
            const previous = index < 0 ? document.activeElement : layers[index].previous;

            if (index >= 0) {
                layers.splice(index, 1);
            }

            if (layers.length === 0) {
                document.addEventListener('keydown', onKeyDown, true);
            }

            layers.push({ element, previous, key });
            document.body.style.overflow = 'hidden';

            focusInto(element);
        },
        release: key => {
            const index = key ? layers.findIndex(layer => layer.key === key) : layers.length - 1;

            if (index < 0) {
                return;
            }

            const [released] = layers.splice(index, 1);
            const layer = current();

            if (layer === null) {
                document.body.style.overflow = '';
                document.removeEventListener('keydown', onKeyDown, true);
            }

            if (released.previous !== null && released.previous.isConnected) {
                released.previous.focus();
            } else if (layer !== null) {
                focusInto(layer.element);
            }
        },
    };
})();
