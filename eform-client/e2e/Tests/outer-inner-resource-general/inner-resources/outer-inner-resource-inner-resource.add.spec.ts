import loginPage from '../../../Page objects/Login.page';
import outerInnerResourceInnerResourcePage from '../../../Page objects/OuterInnerResource/OuterInnerResourceInnerResource.page';
import { generateRandmString } from '../../../Helpers/helper-functions';
import outerInnerResourceModalPage from '../../../Page objects/OuterInnerResource/OuterInnerResourceModal.page';

const expect = require('chai').expect;
const newNameInnerResources = generateRandmString();

describe('Machine Area Area Add', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceInnerResourcePage.goToInnerResource();
  });
  it('should not create a new area without everything', function () {
    const rowNumBeforeCreate = outerInnerResourceInnerResourcePage.rowNum;
    outerInnerResourceInnerResourcePage.openCreateModal();
    expect(
      outerInnerResourceModalPage.innerResourceCreateSaveBtn.isEnabled()
    ).eq(false);
    outerInnerResourceInnerResourcePage.closeCreateModal(true);
    expect(
      outerInnerResourceInnerResourcePage.rowNum,
      'An extra innerResource was created'
    ).eq(rowNumBeforeCreate);
  });
  it('should add area with only name', function () {
    const rowNumBeforeCreate = outerInnerResourceInnerResourcePage.rowNum;
    outerInnerResourceInnerResourcePage.createNewInnerResource(
      newNameInnerResources
    );
    expect(outerInnerResourceInnerResourcePage.rowNum).eq(
      rowNumBeforeCreate + 1
    );
    const listRowObject = outerInnerResourceInnerResourcePage.getInnerObjectByName(
      newNameInnerResources
    );
    expect(listRowObject.name, 'Name in table is incorrect').equal(
      newNameInnerResources
    );
  });
  after('clean up', function () {
    const listRowObject = outerInnerResourceInnerResourcePage.getInnerObjectByName(
      newNameInnerResources
    );
    listRowObject.delete();
  });
});
