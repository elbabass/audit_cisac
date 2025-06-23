---
description: "Permet de faire la synthèse pour un aspect donné en préparation d'un rapport."
---

# Extraction d'informations pour le rapport d'audit

## Prérequis

### Informations à fournir

Avant de commencer les instructions suivantes, il est nécessaire de s'assurer que les documents suivants sont disponibles :

- Quel aspect et informations extraire des notes de réunion et documents de synthèse. Cette information sera appelée *"recherche"* dans la suite de ce document.
- Quel type de document créer (par exemple, un document de synthèse, un rapport d'audit, etc.). Cette information sera appelée *"type de document"* dans la suite de ce document.
- De quels document les informations doivent être extraites. Il peut s'agir de notes de réunion, de documents de synthèse, ou d'autres types de documents. Cette information sera appelée *"documents sources"* dans la suite de ce document.

Si l'une de ces informations n'est pas fournie, il est impossible de procéder à l'extraction des informations. Il faut alors stopper le processus et demander à l'utilisateur de fournir les informations manquantes.

### Documents à utiliser

Sauf si l'utilisateur indique le contraire, il faut utiliser les documents suivants pour l'extraction des informations :

- Le référentiel de classification des informations extraites sera `agile-fluency-questionnaire-ouvert.md` et `agile-fluency-project-ebook-rtw-1.md`. Ce document sera appelé *"référentiel de classification"* dans la suite de ce document.
- Le document contenant le plan de rédaction du rapport d'audit sera `plan-redaction-rapport-audit.md`. Ce document sera appelé *"plan de rédaction"* dans la suite de ce document.
- Le dossier de destination pour le document de synthèse sera `docs/restitution`. Ce dossier sera appelé *"dossier de destination"* dans la suite de ce document.

### Validation des informations entrées

Avant de commencer l'extraction des informations, il faut valider que les informations fournies par l'utilisateur sont correctes. Il faut s'assurer que :

- Le "plan de rédaction"
    - contient une liste d'actions à réaliser
    - chaque action est numérotée de manière unique
- Le "recherche" est présent dans le plan de rédaction. Son identifiant sera appelé *"sujet_id"* dans la suite de ce document.
- Le "référentiel de classification" est composé de plusieurs sections, catégories ou thèmes. Ceux-ci seront appelés *"dimensions"* dans la suite de ce document.

Si l'une de ces validations échoue, il faut stopper le processus et demander à l'utilisateur de corriger les informations fournies.

## Instructions

Extrais les différentes "dimensions" des "documents sources", et classe les selon les "dimensions" du "référentiel de classification". Ajoute les dans un nouveau document dans le "dossier de destination". Le préfixe de ce document sera "sujet_id" en référence à l'élément de "plan de rédaction" correspondant, au lieu de la numérotation habituelle à 3 chiffres.

Pour chaque "dimensions" et élément du "recherche", précise:

- la source : Qui en a parlé, si cette "recherche" est issue d'une interview
- le type : S'il s'agit de processus, rituel, artefact, rôle, cadre de travail, communication, qualité, ingénierie ou autre type de pratique
- le niveau : Si adéquat, le niveau dans le référentiel de classification
- le nom : Le nom de l’élément de "recherche"
- la description : Une description de la manière dont cette "recherche" est appliquée ou présente pour ce projet
