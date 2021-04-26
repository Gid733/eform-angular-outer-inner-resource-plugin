import outerInnerResourceOuterResourcePage, {
  ListRowObject,
} from '../../../Page objects/OuterInnerResource/OuterInnerResourceOuterResource.page';
import loginPage from '../../../Page objects/Login.page';
import { generateRandmString } from '../../../Helpers/helper-functions';

const expect = require('chai').expect;
const newName = generateRandmString();

describe('Machine Area Machine delete', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    outerInnerResourceOuterResourcePage.goToOuterResource();
  });
  it('should add machine', function () {
    outerInnerResourceOuterResourcePage.createNewInnerResource(newName);
  });
  it('should not delete machine', function () {
    const rowNumBeforeDelete = outerInnerResourceOuterResourcePage.rowNum;
    outerInnerResourceOuterResourcePage
      .getOuterObjectByName(newName)
      .delete(true);
    expect(outerInnerResourceOuterResourcePage.rowNum, 'Area is deleted').eq(
      rowNumBeforeDelete
    );
  });
  it('should delete machine', function () {
    const rowNumBeforeDelete = outerInnerResourceOuterResourcePage.rowNum;
    outerInnerResourceOuterResourcePage.getOuterObjectByName(newName).delete();
    expect(
      outerInnerResourceOuterResourcePage.rowNum,
      'Area is not deleted'
    ).eq(rowNumBeforeDelete - 1);
  });
});
