class SubmissionPage {
   static originalTitle = () => cy.get("input[type='text'").first();
   static creatorNameNumberField = (row: number) => cy.get("div[class*='nameNumberContainer").find('input').eq(row - 1);
   static lookupCreatorNameNumber = (row: number) => cy.get("div[class*='nameNumberContainer").find('img').eq(row - 1);
   static addIp = () => cy.get("[class*='IpLookup_grid'] button");
   static workcodeField = () => cy.get("[id='Agency Work Code:']").find('input').first();
   static next = () => cy.contains('Next');
   static disambiguateAllTab = () => cy.contains('Disambiguate all ISWCs');
   static disambiguateReason = () => cy.get('select').first();
   static submit = () => cy.contains('Submit');
   static addNewCreator = () => cy.contains('Add New Creator');
   static assignedIswc = () => cy.get("[id='ISWC:']");
   static removeIp = () => cy.contains('Creators:').parent().find("[alt='Remove Icon']");
   static derivedType = () => cy.get('select').eq(2);
   static derivedIswc = () => cy.get("[id='Derived Work ISWC:']").find('input');
   static derivedTitle = () => cy.get("[id='Derived Work Title:']").find('input');
   static publisherNameNumber = () => cy.contains('Publishers:').parent().find('input');
   static lookupPublisherNameNumber = () => cy.contains('Publishers:').parent().find("[alt='Search icon']");
   static addPublisher = () => cy.contains('Publishers:').parent().contains('Add');
   static performerLastName = () => cy.get("[id='Surname / Band Name:']").find('input');
   static performerFirstName = (i: number) =>
      cy
         .contains('Performers:')
         .parent()
         .find('tr')
         .eq(i + 1)
         .find('td')
         .eq(1)
         .find('input');
   static addNewPerformer = () => cy.get('button').contains('Add New Performer');
   static resubmissionIswc = () => cy.get('[class*=GridTextCell]').eq(0).children().children();
}

export default SubmissionPage;
