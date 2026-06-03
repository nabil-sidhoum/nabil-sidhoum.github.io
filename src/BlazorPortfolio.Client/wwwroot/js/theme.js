export function getTheme() {
    return localStorage.getItem('nabil-portfolio-theme') || 'dark';
}

export function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('nabil-portfolio-theme', theme);
}
