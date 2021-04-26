import { PageWithNavbarPage } from '../PageWithNavbar.page';
import outerInnerResourceInnerResourcePage from './OuterInnerResourceInnerResource.page';
import outerInnerResourceModalPage from './OuterInnerResourceModal.page';

export class OuterInnerResourceOuterResourcePage extends PageWithNavbarPage {
  constructor() {
    super();
  }

  public get rowNum(): number {
    browser.pause(500);
    return $$('#tableBodyOuterResources > tr').length;
  }

  public get outerResourceMenuPoint() {
    const ele = $('#outer-inner-resource-pn-outer-resources');
    ele.waitForDisplayed({ timeout: 20000 });
    ele.waitForClickable({ timeout: 20000 });
    return ele;
  }

  public get newOuterResourceBtn() {
    const ele = $('#newOuterResourceBtn');
    ele.waitForDisplayed({ timeout: 20000 });
    ele.waitForClickable({ timeout: 20000 });
    return ele;
  }

  goToOuterResource() {
    outerInnerResourceInnerResourcePage.outerInnerResourceDropdownMenu.click();
    this.outerResourceMenuPoint.click();
    this.newOuterResourceBtn.waitForDisplayed({ timeout: 20000 });
  }

  getOuterObjectByName(name: string): ListRowObject {
    browser.pause(500);
    for (let i = 1; i < this.rowNum + 1; i++) {
      const listRowObject = this.getEformRowObj(i);
      if (listRowObject.name === name) {
        return listRowObject;
      }
    }
    return null;
  }

  public getEformRowObj(i: number): ListRowObject {
    return new ListRowObject(i);
  }

  openCreateModal(name?: string, externalId?: number | string) {
    this.newOuterResourceBtn.click();
    if (name) {
      outerInnerResourceModalPage.outerResourceCreateNameInput.setValue(name);
    }
    if (externalId) {
      outerInnerResourceModalPage.createOuterResourceExternalId.setValue(
        externalId.toString()
      );
    }
  }

  closeCreateModal(clickCancel = false) {
    if (!clickCancel) {
      outerInnerResourceModalPage.outerResourceCreateSaveBtn.click();
      $('#spinner-animation').waitForDisplayed({
        timeout: 20000,
        reverse: true,
      });
    } else {
      outerInnerResourceModalPage.outerResourceCreateCancelBtn.click();
      this.newOuterResourceBtn.waitForDisplayed({ timeout: 20000 });
    }
  }

  createNewInnerResource(
    name?: string,
    externalId?: number | string,
    clickCancel = false
  ) {
    this.openCreateModal(name, externalId);
    this.closeCreateModal(clickCancel);
  }
}

const outerInnerResourceOuterResourcePage = new OuterInnerResourceOuterResourcePage();
export default outerInnerResourceOuterResourcePage;

export class ListRowObject {
  constructor(rowNum) {
    this.element = $$('#tableBodyOuterResources > tr')[rowNum - 1];
    if (this.element) {
      try {
        this.name = this.element.$('#outerResourceName').getText();
      } catch (e) {}
      try {
        this.id = +this.element.$('#outerResourceId').getText();
      } catch (e) {}
      try {
        this.externalId = +this.element.$('#outerResourceExternalId').getText();
      } catch (e) {}
      try {
        this.updateBtn = this.element.$('#outerResourceEditBtn');
      } catch (e) {}
      try {
        this.deleteBtn = this.element.$('#outerResourceDeleteBtn');
      } catch (e) {}
    }
  }

  public element;
  public id: number;
  public name: string;
  public externalId: number;
  public updateBtn;
  public deleteBtn;

  openDeleteModal() {
    this.deleteBtn.click();
    outerInnerResourceModalPage.outerResourceDeleteDeleteBtn.waitForDisplayed({
      timeout: 20000,
    });
  }

  closeDeleteModal(clickCancel = false) {
    if (!clickCancel) {
      outerInnerResourceModalPage.outerResourceDeleteDeleteBtn.click();
      $('#spinner-animation').waitForDisplayed({
        timeout: 20000,
        reverse: true,
      });
    } else {
      outerInnerResourceModalPage.outerResourceDeleteCancelBtn.click();
      outerInnerResourceOuterResourcePage.newOuterResourceBtn.waitForDisplayed({
        timeout: 20000,
      });
    }
  }

  delete(clickCancel = false) {
    this.openDeleteModal();
    this.closeDeleteModal(clickCancel);
  }

  openEditModal(newName?: string, newExternalId?: number | string) {
    this.updateBtn.click();
    if (newName) {
      outerInnerResourceModalPage.outerResourceEditNameInput.setValue(newName);
    }
    if (newExternalId) {
      outerInnerResourceModalPage.outerResourceEditExternalIdInput.setValue(
        newExternalId.toString()
      );
    }
  }

  closeEditModal(clickCancel = false) {
    if (!clickCancel) {
      outerInnerResourceModalPage.outerResourceEditSaveBtn.click();
      $('#spinner-animation').waitForDisplayed({
        timeout: 20000,
        reverse: true,
      });
    } else {
      outerInnerResourceModalPage.outerResourceEditCancelBtn.click();
      outerInnerResourceOuterResourcePage.newOuterResourceBtn.waitForDisplayed({
        timeout: 20000,
      });
    }
  }

  edit(newName?: string, newExternalId?: number | string, clickCancel = false) {
    this.openEditModal(newName, newExternalId);
    this.closeEditModal(clickCancel);
  }
}
