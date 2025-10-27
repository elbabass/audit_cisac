class ViewMoreGrid {
   static viewMore = () => cy.get("[id='View More']").first();
   static ipNameNumber = () => cy.get('[class*="ViewMore"]').find('[id="IP Name Number:"]');
   static ipAffiliation = () => cy.get('[id="Affiliation:"]');
   static ipBaseNumber = () => cy.get('[id="IP Base Number:"]');
   static ipRole = () => cy.get('[id="Rolled Up Role:"]');
   static ipName = () => cy.get('[class*="ViewMore"]').find('[id="Creator Name(s):"]').find('span');
   static agencyyWorkCode = () => cy.get('[id="Agency Work Code:"]');
   static viewSubmssionHistory = () => cy.get('[id="View Submission History"]').first();
   static deleteWork = () => cy.get('#Delete');
   static confirmDeletion = () => cy.get('.modal-content button').eq(2);
   static disambiguationReason = () => cy.get('[id="Reason:"]');
   static disambiguatedFrom = () => cy.get('[id="Disambiguated From ISWCs:"]');
   static performersSurname = () => cy.get('[id="Surname / Band Name:"]');
   static agencyCode = () => cy.get('[id="Agency Code:"]');
   static addToMergeList = () => cy.get("input[type='checkbox']");
   static viewMergeList = () => cy.get('[id="View Merge List"]');
   static demerge = () => cy.get('[id="Demerge here"]').first();
   static updateSubmission = () => cy.get("[id='Update Submission']").first();
   static newSubmission = () => cy.get("[id='New Submission']").first();
   static derivedFromTitle = () => cy.contains('Derived Works:').parent().find("[id='Title:']");
   static derivedFromIswc = () => cy.contains('Derived Works:').parent().find('span').eq(1);
   static derivedFromType = () => cy.contains('Derived Works:').parent().find("[id='Type:']");
   static archivedIswcs = () => cy.contains('Archived ISWCs').parent();
   static performerLastName = () => cy.get("[id='Surname / Band Name:']").find('span');
   static performerFirstName = (i: number) =>
      cy
         .contains('Performers:')
         .parent()
         .find('tr')
         .eq(i + 1)
         .find('td')
         .eq(3)
         .find('span');
   static deleteAndResubmit = () => cy.get("[id='Delete & Resubmit']").eq(2);
   static confirmDeleteAndResubmit = () => cy.window().get('[class*=ActionButton_actionButton__23QOh]').filter(((index, el) => el.innerText === "Confirm Deletion"));
}

export default ViewMoreGrid;
