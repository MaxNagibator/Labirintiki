window.labirintUpdate = (() => {
    const checkInterval = 30 * 60 * 1000;

    let registration = null;
    let listener = null;
    let isReady = false;

    const notify = () => {
        isReady = true;
        listener?.invokeMethodAsync('OnUpdateReady');
    };

    const track = worker => {
        if (worker === null) {
            return;
        }

        worker.addEventListener('statechange', () => {
            if (worker.state === 'installed' && navigator.serviceWorker.controller !== null) {
                notify();
            }
        });
    };

    const check = () => registration?.update().catch(() => { });

    return {
        register: () => {
            if ('serviceWorker' in navigator === false) {
                return;
            }

            navigator.serviceWorker.register('service-worker.js', { updateViaCache: 'none' })
                .then(value => {
                    registration = value;

                    if (registration.waiting !== null && navigator.serviceWorker.controller !== null) {
                        notify();
                    }

                    track(registration.installing);
                    registration.addEventListener('updatefound', () => track(registration.installing));

                    setInterval(check, checkInterval);
                    document.addEventListener('visibilitychange', () => document.hidden || check());
                })
                .catch(error => console.error('Service worker: регистрация не удалась', error));
        },
        watch: reference => {
            listener = reference;

            if (isReady) {
                notify();
            }
        },
        apply: () => {
            const waiting = registration?.waiting;

            if (!waiting) {
                location.reload();
                return;
            }

            navigator.serviceWorker.addEventListener('controllerchange', () => location.reload(), { once: true });
            waiting.postMessage({ type: 'SKIP_WAITING' });
        },
    };
})();

window.labirintUpdate.register();
