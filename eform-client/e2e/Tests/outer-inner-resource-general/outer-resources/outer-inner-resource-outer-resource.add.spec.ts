import outerInnerResourceOuterResourcePage from '../../../Page objects/OuterInnerResource/OuterInnerResourceOuterResource.page';
import outerInnerResourceModalPage from '../../../Page objects/OuterInnerResource/OuterInnerResourceModal.page';
import loginPage from '../../../Page objects/Login.page';
import { generateRandmString } from '../../../Helpers/helper-functions';

const expect = require('chai').expect;
const newName = generateRandmString();

describe('Machine Area Machine Add', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceOuterResourcePage.goToOuterResource();
  });
  it('should add machine with only name', function () {
    const rowNumBeforeCreate = outerInnerResourceOuterResourcePage.rowNum;
    outerInnerResourceOuterResourcePage.createNewInnerResource(newName);
    expect(outerInnerResourceOuterResourcePage.rowNum).eq(
      rowNumBeforeCreate + 1
    );
    const listRowObject = outerInnerResourceOuterResourcePage.getOuterObjectByName(
      newName
    );
    expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  });
  it('should not create machine without name', function () {
    const rowNumBeforeCreate = outerInnerResourceOuterResourcePage.rowNum;
    outerInnerResourceOuterResourcePage.openCreateModal();
    expect(
      outerInnerResourceModalPage.outerResourceCreateSaveBtn.isEnabled()
    ).eq(false);
    outerInnerResourceOuterResourcePage.closeCreateModal(true);
    expect(
      outerInnerResourceOuterResourcePage.rowNum,
      'An extra outerResource was created'
    ).eq(rowNumBeforeCreate);
  });
  after('clean up', function () {
    outerInnerResourceOuterResourcePage.getOuterObjectByName(newName).delete();
  });
});
