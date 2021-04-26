import outerInnerResourceInnerResourcePage, {
  ListRowObject,
} from '../../../Page objects/OuterInnerResource/OuterInnerResourceInnerResource.page';
import outerInnerResourceModalPage from '../../../Page objects/OuterInnerResource/OuterInnerResourceModal.page';
import loginPage from '../../../Page objects/Login.page';
import { generateRandmString } from '../../../Helpers/helper-functions';

const expect = require('chai').expect;
const nameInnerResource = generateRandmString();

describe('Machine Area Area delete', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceInnerResourcePage.goToInnerResource();
  });
  it('should create a new area', function () {
    outerInnerResourceInnerResourcePage.createNewInnerResource(
      nameInnerResource
    );
  });
  it('should not delete area', function () {
    const rowNumBeforeDelete = outerInnerResourceInnerResourcePage.rowNum;
    const listRowObject = outerInnerResourceInnerResourcePage.getInnerObjectByName(
      nameInnerResource
    );
    listRowObject.delete(true);
    expect(outerInnerResourceInnerResourcePage.rowNum, 'Area is deleted').eq(
      rowNumBeforeDelete
    );
  });
  it('should delete area', function () {
    const rowNumBeforeDelete = outerInnerResourceInnerResourcePage.rowNum;
    const listRowObject = outerInnerResourceInnerResourcePage.getInnerObjectByName(
      nameInnerResource
    );
    listRowObject.delete();
    expect(
      outerInnerResourceInnerResourcePage.rowNum,
      'Area is not deleted'
    ).eq(rowNumBeforeDelete - 1);
  });
});
