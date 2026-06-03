let observer = null;

export function init(dotnetRef) {
    const sections = document.querySelectorAll('section[id]');
    if (sections.length === 0) { return; }

    observer = new IntersectionObserver(
        (entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    dotnetRef.invokeMethodAsync('OnSectionVisible', entry.target.id);
                }
            });
        },
        { rootMargin: '-20% 0px -70% 0px', threshold: 0 }
    );

    sections.forEach(s => observer.observe(s));
}

export function dispose() {
    if (observer) {
        observer.disconnect();
        observer = null;
    }
}
