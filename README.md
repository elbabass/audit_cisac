# Réponse à l'appel d'offre d'audit CISAC

Suivi et réponse à l'appel d'offre d'audit CISAC, réalisé par PALO IT.

## Project Purpose

Ce repository contient les ressources pour le projet d'audit CISAC.

## Structure

- `docs/` — Main documentation folder
  - `index.md` — Entry point for the documentation
  - `css/` — Custom stylesheets
  - `docs_de_travail/` — Working documents
  - `img/` — Images and logos
  - `reponse/` — Detailed response sections
  - `resources/` — Reference materials and resources
- `mkdocs.yml` — MkDocs configuration file
- `pyproject.toml` — Python project configuration file

## Getting Started

1. **Create a virtual environment** (using [uv](https://github.com/astral-sh/uv)):

   ```sh
   uv venv
   ```

2. **Activate the virtual environment:**

   - PowerShell:

     ```powershell
     .venv\Scripts\Activate.ps1
     ```

   - Command Prompt (cmd):

     ```cmd
     .venv\Scripts\activate.bat
     ```

   - Bash/Zsh (Git Bash, WSL, macOS, Linux):

     ```sh
     source .venv/bin/activate
     ```

3. **Install dependencies from pyproject.toml:**

   ```sh
   uv pip install -r pyproject.toml
   ```

4. **Serve documentation locally:**

   ```sh
   mkdocs serve
   ```

   Then open [http://127.0.0.1:8000](http://127.0.0.1:8000) in your browser.

## Project Navigation

- Project summary and diagrams are in `docs/`
- Meeting notes and resources are in `docs/resources/`

---

For more details, see the navigation in `mkdocs.yml`.

## Contributing

We welcome contributions to improve the documentation. Please refer to the `CONTRIBUTING.md` file for guidelines on how to contribute.

## Code of Conduct

Please adhere to the [Code of Conduct](CODE_OF_CONDUCT.md) to ensure a welcoming environment for all contributors.

## License

This project is licensed for internal use only. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please contact the maintainers at [bgallay@palo-it.com].

---

Note: On Windows, always activate the virtual environment before running mkdocs or installing packages.
