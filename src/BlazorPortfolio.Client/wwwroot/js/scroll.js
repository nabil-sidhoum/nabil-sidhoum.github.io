window.scrollHelper = {
    init: function () {
        const mybutton = document.getElementById("topbtn");
        const mainContent = document.querySelector(".main-content");

        if (!mybutton || !mainContent) {
            console.error("❌ Élément introuvable !");
            return;
        }

        // On cache le bouton au départ
        mybutton.style.display = "none";

        // ✅ Ici on écoute le scroll de main-content (pas window)
        mainContent.addEventListener("scroll", () => {
            if (mainContent.scrollTop > 20) {
                mybutton.style.display = "grid";
            } else {
                mybutton.style.display = "none";
            }
        });

        // Scroll vers le haut en douceur
        mybutton.addEventListener("click", () => {
            mainContent.scrollTo({ top: 0, behavior: "smooth" });
        });
    }
};
