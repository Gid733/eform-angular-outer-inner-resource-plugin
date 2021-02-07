import outerInnerResourceInnerResourcePage , {ListRowObject} from '../../../Page objects/outer-inner-resource/outer-inner-resource-inner-resource.page';
import outerInnerResourceModalPage from '../../../Page objects/outer-inner-resource/outer-inner-resource-modal.page';
import loginPage from '../../../Page objects/Login.page';
import {generateRandmString} from '../../../Helpers/helper-functions';

const expect = require('chai').expect;
const nameInnerResource = generateRandmString();

describe('Machine Area Area delete', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceInnerResourcePage.goToInnerResource();
  });
  it('should create a new area', function () {
    outerInnerResourceInnerResourcePage.createNewInnerResource(nameInnerResource);
  });
  it('should delete area', function () {
    const rowNumBeforeDelete = outerInnerResourceInnerResourcePage.rowNum;
    const listRowObject = outerInnerResourceInnerResourcePage.getInnerObjectByName(nameInnerResource);
    listRowObject.delete();
    expect(outerInnerResourceInnerResourcePage.rowNum, 'Area is not deleted').eq(rowNumBeforeDelete - 1);
  });
});
