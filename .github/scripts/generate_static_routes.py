"""Génère une page HTML statique par route du blog dans le dossier publié.

GitHub Pages ne connaît pas les routes Blazor : sans fichier pour /blog ou
/blog/<slug>, il sert 404.html avec le code HTTP 404. Le navigateur affiche
quand même la page (Blazor reprend la main), mais les aperçus de liens
(LinkedIn, Slack) et les moteurs de recherche voient une page introuvable.

Ce script copie index.html pour chaque route, avec ses propres balises
<title>, description, og:* et twitter:* :
  - blog.html et blog/index.html  -> /blog (200, direct ou après 301 vers /blog/)
  - blog/<slug>.html              -> /blog/<slug> (200 direct, sans redirection)

Usage : python3 generate_static_routes.py <dossier wwwroot publié>
"""

import html
import json
import re
import sys
from pathlib import Path

SITE_URL = "https://nabil-sidhoum.github.io"
AUTHOR = "Nabil Sidhoum"
BLOG_TITLE = "Blog · " + AUTHOR
BLOG_DESCRIPTION = "Articles de " + AUTHOR + ", Tech Lead .NET : architecture, IA, CI/CD et retours d'expérience."

# Même contrainte que BlogService : le slug devient un nom de fichier,
# [a-z0-9-] empêche tout path-traversal (« ../ »).
SLUG_PATTERN = re.compile(r"^[a-z0-9-]+$")


def replace_once(page, pattern, value):
    """Remplace le contenu d'une balise ; échoue si elle n'existe pas exactement une fois."""
    escaped = html.escape(value, quote=True)
    result, count = re.subn(pattern, lambda match: match.group(1) + escaped + match.group(2), page)
    if count != 1:
        raise ValueError("balise attendue une fois, trouvée " + str(count) + " fois : " + pattern)
    return result


def render(template, title, description, url, og_type):
    page = replace_once(template, r"(<title>)[^<]*(</title>)", title)
    page = replace_once(page, r'(<meta name="description" content=")[^"]*(")', description)
    page = replace_once(page, r'(<meta property="og:title" content=")[^"]*(")', title)
    page = replace_once(page, r'(<meta property="og:description" content=")[^"]*(")', description)
    page = replace_once(page, r'(<meta property="og:url" content=")[^"]*(")', url)
    page = replace_once(page, r'(<meta property="og:type" content=")[^"]*(")', og_type)
    page = replace_once(page, r'(<meta name="twitter:title" content=")[^"]*(")', title)
    page = replace_once(page, r'(<meta name="twitter:description" content=")[^"]*(")', description)
    return page


def write(path, content):
    path.parent.mkdir(parents=True, exist_ok=True)
    with open(path, "w", encoding="utf-8", newline="") as file:
        file.write(content)
    print("généré : " + str(path))


def main():
    if len(sys.argv) != 2:
        sys.exit("usage : generate_static_routes.py <dossier wwwroot publié>")

    root = Path(sys.argv[1])
    with open(root / "index.html", encoding="utf-8", newline="") as file:
        template = file.read()
    with open(root / "posts" / "index.json", encoding="utf-8") as file:
        articles = json.load(file)["articles"]

    blog_page = render(template, BLOG_TITLE, BLOG_DESCRIPTION, SITE_URL + "/blog", "website")
    write(root / "blog.html", blog_page)
    write(root / "blog" / "index.html", blog_page)

    for article in articles:
        slug = article["Slug"]
        if not SLUG_PATTERN.fullmatch(slug):
            sys.exit("slug invalide dans posts/index.json : " + repr(slug))
        page = render(
            template,
            article["Title"] + " · " + AUTHOR,
            article["Summary"],
            SITE_URL + "/blog/" + slug,
            "article")
        write(root / "blog" / (slug + ".html"), page)


if __name__ == "__main__":
    main()
