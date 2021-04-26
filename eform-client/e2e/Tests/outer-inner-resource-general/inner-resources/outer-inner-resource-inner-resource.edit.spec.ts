import outerInnerResourceInnerResourcePage, {
  ListRowObject,
} from '../../../Page objects/OuterInnerResource/OuterInnerResourceInnerResource.page';
import outerInnerResourceModalPage from '../../../Page objects/OuterInnerResource/OuterInnerResourceModal.page';
import loginPage from '../../../Page objects/Login.page';
import { generateRandmString } from '../../../Helpers/helper-functions';

const expect = require('chai').expect;
const newName = generateRandmString();

describe('Machine Area Area edit', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceInnerResourcePage.goToInnerResource();
  });
  it('should create a new area', function () {
    outerInnerResourceInnerResourcePage.createNewInnerResource(newName);
  });
  // TODO Can't change name.
  // it('should edit area', function () {
  //   const listRowObject = new ListRowObject(outerInnerResourceInnerResourcePage.rowNum());
  //   const newName = 'New Name';
  //   listRowObject.updateBtn.click();
  //   $('#updateInnerResourceName').waitForDisplayed({timeout: 20000});
  //   outerInnerResourceModalPage.innerResourceEditName.clearElement();
  //   outerInnerResourceModalPage.innerResourceEditName.addValue(newName);
  //   outerInnerResourceModalPage.innerResourceEditSaveBtn.click();
  //   $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  //   browser.refresh();
  //   browser.waitForVisible(listRowObject.updateBtn, 20000);
  //   expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  // });
  after('clean up', function () {
    const listRowObject = outerInnerResourceInnerResourcePage.getInnerObjectByName(
      newName
    );
    listRowObject.delete();
  });
});
