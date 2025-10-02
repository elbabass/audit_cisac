---
description: "Révision et complétion des notes d'interview à base d'Agile Fluency."

---

# Révision des notes d'interview

## Prérequis

Avant de commencer, assurez-vous que les éléments suivants sont disponibles :

- L'utilisateur a fournit le document de notes d'interview `NOTES_INTERVIEW`
- L'utilisateur a fournit le fichier de transcription `FICHIERS_INTERVIEW_TRANSCRIPT`
- L'utilisateur a fourni l'heure de début de la transcription `HEURE_DEBUT_TRANSCRIPTION`

Si l'un de ces éléments est manquant, arrêtez le processus et demandez à l'utilisateur de fournir les informations nécessaires.

## Instructions

Dans le document de notes d'interview *NOTES_INTERVIEW*, re-ventile les questions auxquelles l'interviewé a répondu de la manière suivante:

1. Parcours des documents source:
    - Utilise `docs/docs_de_travail/template-interview.md` pour repérer les questions et parties de documents qui ne sont pas propres à cette interview
    - Utilise les annotations de temps au format `{hh}h{mm}` où `{hh}` est l'heure et `{mm}` les minutes pour retrouver à peu près le moment de l'interview dans la transcriptions *FICHIERS_INTERVIEW_TRANSCRIPT*
    - l'horodatage du transcript a commencé à *HEURE_DEBUT_TRANSCRIPTION*. L'horodatage 00:00 est donc *HEURE_DEBUT_TRANSCRIPTION* dans les notes d'interview
    - Utilise `docs/docs_de_travail/agile-fluency-questionnaire-ouvert.md`  et `docs/resources/agile-fluency/md/agile-fluency-project-ebook-rtw-1/agile-fluency-project-ebook-rtw-1.md` comme référence pour mieux comprendre les thématiques et questions
2. Création du document cible *NOTES_INTERVIEW_REVISION*:
    - Le document cible doit être ajouté dans le même dossier que le document de notes d'interview *NOTES_INTERVIEW*
    - le nom du document cible doit être le même que le document de notes d'interview *NOTES_INTERVIEW* mais avec le suffixe `-revision` ajouté avant l'extension `.md`
    - Ajoute les notes manquantes ou complète les notes existantes
    - Re-ventile ou crée des renvois au sein du document lorsque des notes répondent à une autre question "Agile Fluency"

## Vérifications

- Aucune note ne doit être ajoutée si elle n'est pas présente dans le document original ou dans la transcription
- Le document cible *NOTES_INTERVIEW_REVISION* doit être créé dans le même dossier que le document de notes d'interview *NOTES_INTERVIEW*
- Le nom du document cible doit être le même que le document de notes d'interview *NOTES_INTERVIEW* mais avec le suffixe `-revision` ajouté avant l'extension `.md`
- Les horodatages dans le document cible doivent correspondre à ceux du document original, en tenant compte de l'offset de *HEURE_DEBUT_TRANSCRIPTION*.