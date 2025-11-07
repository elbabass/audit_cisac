# Presentation generation - prompt history

## 1. Initial prompt

Plan mode

```markdown
#  First conclusions presentation - draft audit restitution

  Prepare a presentation document that presents the draft audit restitution for the first conclusions of the project. It outlines the key findings, facts, recommendations and next steps based on the audit conducted.

  ## I. Overview of the Audit History

  - The initial description of the audit, including its purpose, scope, and methodology are detailed in `docs/proposal/` documents
  - The relaunch of the audit is described in `docs/meetings/20251007-recadrage.md` and `docs/meetings/2025-10-07-NotesGuillaume.txt`
  - Outlines in the first workshops relates the first goals and approach in `docs/meetings/2025-10-20-NotesGuillaume.txt`, `docs/meetings/20251020-ISWC Audit - Workshop 1.txt` and
  `docs/meetings/20251020-SpanishPoint-AuditRelaunch.md`
  - An important pivot was discussed in the transcript `docs/meetings/20251021-ISWC - Discussion Yann_Guillaume_Bastien.txt`
  - Audit status describe how the approach plan and the goals evolved in `docs/project_management/*-AuditStatus*.md`
  - Audit of documentation, infrastructure and codebase are detailed in `docs/work_in_progress/**/*`. **Information from these documents may contain some errors, bias and hallucinations and should be verified before use.**

  ## II. Broad recommendations

  - (To be elaborated after further analysis of the audit findings)Code and technical infrastructure improvements
    - Duplicates, dead code removal, refactoring
    - Test coverage improvement and tend to create meaningful automated tests
  - Improve vendor/client relationship management : focus on shared understanding, transparency and collaboration
    - Review existing documentation incrementally and collaboratively with the vendor to achieve living documentation
    - Share from both sides some strong and common operational artefacts to support usual collaboration (e.g. architecture diagrams, non-regression test suites, deployment procedures, runbooks)
    - Establish common dashboards and monitoring tools with a few key metrics agreed between both parties, always accessible and reviewed regularly
  - Renegotiate contracts and review delegation of responsibilities to ensure transferability and accountability - as if CISAC would quit Spanish Point and take over the system maintenance
    - Enable a full code and infrastructure access for CISAC or a third party, with clear rights and onboarding procedures
    - Define clear SLAs and KPIs for system performance, maintenance and support (bi-lateral agreement more than additional constraints)
    - Integrate some component replacement simulations in the maintenance plan to ensure system resilience and adaptability (elaborate this particular point based on audit findings)

  ## III. Instructions

  Your goal is to create the outlines of the presentation document, that will be the support for a 1.5 hours discussion with CISAC. This will include:

  - Section titles and content summaries
  - First draft of slides breakdown

  1. Review the documents in section I to understand the audit history and context.
  2. Review additional documents if required after first analysis.
  3. Synthesize the key findings, facts, and recommendations from the audit into a coherent narrative for the presentation.
  4. Create a new document to describe the presentation structure and key messages.
  5. Write the presentation plan in the document.
  6. Develop the presentation slides based on the outline and key messages.
     1. Slides must have clear place-holder when an element needs clarification.
     2. Include visuals and diagrams to support the key messages and findings.
     3. Each section must invite discussion and questions from the audience.
     4. Slides can include open questions to encourage audience engagement.
  7. Extract key quotes and data points from the audit findings to support the presentation.
  8. Add speaker notes to elaborate the key findings, quotes and reference of the slide content.
  9. Review and rehearse the presentation to ensure clarity and coherence.
  10. Create a clear summary of the presentation content and key messages for the user:
      1. What are the main findings and recommendations from the audit?
      2. What needs confirmation from the user?
      3. What needs further exploration or clarification?
 
 ## IV. Validation

  - [ ] Review the presentation content and key messages with the user for feedback and validation.
  - [ ] Ensure that all the facts used are clear, based on proof from the audit findings; otherwise, they should be highlighted in the summary for user to take action
  - [ ] Each fact should be verifiable and supported by evidence from the audit; a reference link to the original document, and maybe a quote, should be included in the slide itself or speaker notes.
  - [ ] Evaluate the size of the presentation to be sure that it can be delivered within the allocated time - about 45 minutes, in order to have discussion time.
  - [ ] Ensure that the presentation is engaging and encourages participation from the audience.
  - [ ] Document summary includes key findings, confirmation, and further exploration points.
```

Review Claude's propositions, then accept.

## 2. API error from claude : exeeds 32k tokens

```markdown
Please, continue. Split the content in order to remain ok with the  32000 output token maximum
```

## 3. Additional prompt during generation / planning

```markdown
While reviewing, I think that some Lean principles could be quoted for Vendor lock-in related recommandations. I mean, for instance  "from vendor to long term partnership". This could be useful to CISAC, to see if this kind of change is feasible with SP 
```

## 4. First batch finished, until slide 20, without review

```markdown
For now, this is very good, it really reflects the overall sentiments and historical journey of this audit, for me, and I hope for Guillaume and the client. I see some elements that may have to be double-checked or mitigated. Also, I think Guillaume and I will merge some slides or decide to make some of them optional to be sure we can go through the presentatoin AND the discussions. Continue on the same line of conduct for the creation. For the diagrams, I think an alternate mermaid version can be useful (keep the ASCII version when you do so)
```
