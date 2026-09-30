window.labirintPage = {
    isHidden: () => document.hidden,
    isMotionReduced: () => window.matchMedia('(prefers-reduced-motion: reduce)').matches,
};
