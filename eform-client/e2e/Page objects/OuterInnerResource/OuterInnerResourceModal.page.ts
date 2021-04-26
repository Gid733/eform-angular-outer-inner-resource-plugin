import Page from '../Page';

export class OuterInnerResourceModalPage extends Page {
  constructor() {
    super();
  }

  public get outerResourceCreateNameInput() {
    const ele = $('#createOuterResourceName');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get outerResourceCreateSaveBtn() {
    const ele = $('#outerResourceCreateSaveBtn');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }

  public get outerResourceCreateCancelBtn() {
    const ele = $('#outerResourceCreateCancelBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get outerResourceEditNameInput() {
    const ele = $('#updateOuterResourceName');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get outerResourceEditSaveBtn() {
    const ele = $('#outerResourceEditSaveBtn');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }

  public get outerResourceEditCancelBtn() {
    const ele = $('#outerResourceEditCancelBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  // public get outerResourceDeleteAreaId() {
  //   const ele = $('#selectedOuterResourceId');
  //   ele.waitForDisplayed({timeout: 20000});
  //   ele.waitForClickable({timeout: 20000});
  //   return ele;
  // }

  // public get outerResourceDeleteAreaName() {
  //   const ele = $('#selectedOuterResourceName');
  //   ele.waitForDisplayed({timeout: 20000});
  //   ele.waitForClickable({timeout: 20000});
  //   return ele;
  // }

  public get outerResourceDeleteDeleteBtn() {
    const ele = $('#outerResourceDeleteDeleteBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get outerResourceDeleteCancelBtn() {
    const ele = $('#outerResourceDeleteCancelBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get innerResourceCreateNameInput() {
    const ele = $('#createInnerResourceName');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get innerResourceCreateSaveBtn() {
    const ele = $('#innerResourceCreateSaveBtn');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }

  public get innerResourceCreateCancelBtn() {
    const ele = $('#innerResourceCreateCancelBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get innerResourceEditName() {
    const ele = $('#updateInnerResourceName');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get innerResourceEditSaveBtn() {
    const ele = $('#innerResourceEditSaveBtn');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }

  public get innerResourceEditCancelBtn() {
    const ele = $('#innerResourceEditCancelBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  // public get innerResourceDeleteName() {
  //   const ele = $('#innerResourceDeleteName');
  //   ele.waitForDisplayed({timeout: 20000});
  //   ele.waitForClickable({timeout: 20000});
  //   return ele;
  // }

  public get innerResourceDeleteDeleteBtn() {
    const ele = $('#innerResourceDeleteDeleteBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get innerResourceDeleteCancelBtn() {
    const ele = $('#innerResourceDeleteCancelBtn');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get innerResourceEditExternalIdInput() {
    const ele = $('#updateInnerResourceExternalId');
    ele.waitForDisplayed({timeout: 20000});
    ele.waitForClickable({timeout: 20000});
    return ele;
  }

  public get createInnerResourceId() {
    const ele = $('#createInnerResourceId');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }

  public get createOuterResourceExternalId() {
    const ele = $('#createOuterResourceExternalId');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }

  public get outerResourceEditExternalIdInput() {
    // const ele = $('#createOuterResourceExternalId');
    // ele.waitForDisplayed({timeout: 20000});
    // return ele;
    return this.createOuterResourceExternalId;
  }
}

const outerInnerResourceModalPage = new OuterInnerResourceModalPage();
export default outerInnerResourceModalPage;
