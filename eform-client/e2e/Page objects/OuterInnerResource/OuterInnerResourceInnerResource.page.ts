import { PageWithNavbarPage } from '../PageWithNavbar.page';
import outerInnerResourceModalPage from './OuterInnerResourceModal.page';

export class OuterInnerResourceInnerResourcePage extends PageWithNavbarPage {
  constructor() {
    super();
  }

  public get rowNum(): number {
    browser.pause(500);
    return $$('#tableBodyInnerResources > tr').length;
  }

  public get outerInnerResourceDropdownMenu() {
    const ele = $('#outer-inner-resource-pn');
    ele.waitForDisplayed({ timeout: 20000 });
    ele.waitForClickable({ timeout: 20000 });
    return ele;
  }

  public get innerResourceMenuPoint() {
    const ele = $('#outer-inner-resource-pn-inner-resources');
    ele.waitForDisplayed({ timeout: 20000 });
    ele.waitForClickable({ timeout: 20000 });
    return ele;
  }

  public get newInnerResourceBtn() {
    const ele = $('#newInnerResource');
    ele.waitForDisplayed({ timeout: 20000 });
    ele.waitForClickable({ timeout: 20000 });
    return ele;
  }

  goToInnerResource() {
    this.outerInnerResourceDropdownMenu.click();
    this.innerResourceMenuPoint.click();
    this.newInnerResourceBtn.waitForDisplayed({ timeout: 20000 });
  }

  getInnerObjectByName(name: string) {
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
    this.newInnerResourceBtn.click();
    if (name) {
      outerInnerResourceModalPage.innerResourceCreateNameInput.setValue(name);
    }
    if (externalId) {
      outerInnerResourceModalPage.createInnerResourceId.setValue(
        externalId.toString()
      );
    }
  }

  closeCreateModal(clickCancel = false) {
    if (!clickCancel) {
      outerInnerResourceModalPage.innerResourceCreateSaveBtn.click();
      $('#spinner-animation').waitForDisplayed({
        timeout: 20000,
        reverse: true,
      });
    } else {
      outerInnerResourceModalPage.innerResourceCreateCancelBtn.click();
      this.newInnerResourceBtn.waitForDisplayed({ timeout: 20000 });
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

const outerInnerResourceInnerResourcePage = new OuterInnerResourceInnerResourcePage();
export default outerInnerResourceInnerResourcePage;

export class ListRowObject {
  constructor(rowNum) {
    this.element = $$('#tableBodyInnerResources > tr')[rowNum - 1];
    if (this.element) {
      try {
        this.id = +this.element.$('#innerResourceId').getText();
      } catch (e) {}
      try {
        this.name = this.element.$('#innerResourceName').getText();
      } catch (e) {}
      try {
        this.externalId = +this.element.$('#innerResourceExternalId').getText();
      } catch (e) {}
      try {
        this.updateBtn = this.element.$('#innerResourceEditBtn');
      } catch (e) {}
      try {
        this.deleteBtn = this.element.$('#innerResourceDeleteBtn');
      } catch (e) {}
    }
  }

  public id: number;
  public name: string;
  public externalId: number;
  public element;
  public updateBtn;
  public deleteBtn;

  openDeleteModal() {
    this.deleteBtn.click();
    outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.waitForDisplayed({
      timeout: 20000,
    });
  }

  closeDeleteModal(clickCancel = false) {
    if (!clickCancel) {
      outerInnerResourceModalPage.innerResourceDeleteDeleteBtn.click();
      $('#spinner-animation').waitForDisplayed({
        timeout: 20000,
        reverse: true,
      });
    } else {
      outerInnerResourceModalPage.innerResourceDeleteCancelBtn.click();
      outerInnerResourceInnerResourcePage.newInnerResourceBtn.waitForDisplayed({
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
      outerInnerResourceModalPage.innerResourceEditName.setValue(newName);
    }
    if (newExternalId) {
      outerInnerResourceModalPage.innerResourceEditExternalIdInput.setValue(
        newExternalId.toString()
      );
    }
  }

  closeEditModal(clickCancel = false) {
    if (!clickCancel) {
      outerInnerResourceModalPage.innerResourceEditSaveBtn.click();
      $('#spinner-animation').waitForDisplayed({
        timeout: 20000,
        reverse: true,
      });
    } else {
      outerInnerResourceModalPage.innerResourceEditCancelBtn.click();
      outerInnerResourceInnerResourcePage.newInnerResourceBtn.waitForDisplayed({
        timeout: 20000,
      });
    }
  }

  edit(newName?: string, newExternalId?: number | string, clickCancel = false) {
    this.openEditModal(newName, newExternalId);
    this.closeEditModal(clickCancel);
  }
}
