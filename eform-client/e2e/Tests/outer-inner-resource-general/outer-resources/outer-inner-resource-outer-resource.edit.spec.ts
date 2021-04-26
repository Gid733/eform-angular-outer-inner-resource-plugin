import outerInnerResourceOuterResourcePage, {
  ListRowObject,
} from '../../../Page objects/OuterInnerResource/OuterInnerResourceOuterResource.page';
import outerInnerResourceModalPage from '../../../Page objects/OuterInnerResource/OuterInnerResourceModal.page';
import loginPage from '../../../Page objects/Login.page';
import { generateRandmString } from '../../../Helpers/helper-functions';

const expect = require('chai').expect;
const newName = generateRandmString();

describe('Machine Area Machine edit', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceOuterResourcePage.goToOuterResource();
  });
  it('should add machine', function () {
    outerInnerResourceOuterResourcePage.createNewInnerResource(newName);
  });
  // TODO can't change name.
  // it('should edit machine', function () {
  //   const listRowObject = new ListRowObject(outerInnerResourceOuterResourcePage.rowNum());
  //   const newName = 'New Name';
  //   listRowObject.updateBtn.click();
  //   browser.waitForVisible('#updateMachineName');
  //   outerInnerResourceModalPage.outerResourceEditNameInput.clearElement();
  //   outerInnerResourceModalPage.outerResourceEditNameInput.addValue(newName);
  //   outerInnerResourceModalPage.outerResourceEditSaveBtn.click();
  //   $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  //   browser.refresh();
  //   browser.waitForVisible(listRowObject.updateBtn, 20000);
  //   expect(listRowObject.name, 'Name in table is incorrect').equal(newName);
  // });
  after('should delete machine', function () {
    const rowNumBeforeDelete = outerInnerResourceOuterResourcePage.rowNum;
    outerInnerResourceOuterResourcePage.getOuterObjectByName(newName).delete();
    expect(
      outerInnerResourceOuterResourcePage.rowNum,
      'Area is not deleted'
    ).eq(rowNumBeforeDelete - 1);
  });
});
