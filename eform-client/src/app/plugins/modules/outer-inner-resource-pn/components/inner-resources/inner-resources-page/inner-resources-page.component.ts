import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  InnerResourcesPnModel,
  OuterResourcesPnModel,
  OuterResourcesPnRequestModel,
  InnerResourcePnModel,
} from '../../../models';
import {
  OuterInnerResourcePnOuterResourceService,
  OuterInnerResourcePnInnerResourceService,
} from '../../../services';
import { AuthService } from 'src/app/common/services';
import { PluginClaimsHelper } from 'src/app/common/helpers';
import { OuterInnerResourcePnClaims } from '../../../enums';
import { Subscription } from 'rxjs';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { InnerResourcesStateService } from '../state/inner-resources-state-service';
import { TableHeaderElementModel } from 'src/app/common/models';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machines-page',
  templateUrl: './inner-resources-page.component.html',
  styleUrls: ['./inner-resources-page.component.scss'],
})
export class InnerResourcesPageComponent implements OnInit, OnDestroy {
  @ViewChild('createMachineModal', { static: false }) createMachineModal;
  @ViewChild('editMachineModal', { static: false }) editMachineModal;
  @ViewChild('deleteMachineModal', { static: false }) deleteMachineModal;
  machinesModel: InnerResourcesPnModel = new InnerResourcesPnModel();
  mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel();
  name: string;

  getAllMachinesSub$: Subscription;
  getAllAreasSub$: Subscription;

  tableHeaders: TableHeaderElementModel[] = [
    { name: 'Id', elementId: 'idTableHeader', sortable: true },
    { name: 'Name', elementId: 'nameTableHeader', sortable: true },
    {
      name: 'ExternalId',
      elementId: 'externalIdTableHeader',
      sortable: true,
      visibleName: 'External ID',
    },
    { name: 'Actions', elementId: '', sortable: false },
  ];

  get pluginClaimsHelper() {
    return PluginClaimsHelper;
  }

  get outerInnerResourcePnClaims() {
    return OuterInnerResourcePnClaims;
  }

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    private authService: AuthService,
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    public innerResourcesStateService: InnerResourcesStateService
  ) {}

  get currentRole(): string {
    return this.authService.currentRole;
  }

  ngOnInit() {
    this.getAllInitialData();
  }

  ngOnDestroy() {}

  getAllInitialData() {
    this.getAllMachines();
    this.getMappedAreas();
  }

  getAllMachines() {
    this.getAllMachinesSub$ = this.innerResourcesStateService
      .getAllMachines()
      .subscribe((data) => {
        if (data && data.success) {
          this.machinesModel = data.model;
        }
      });
  }

  getMappedAreas() {
    this.getAllAreasSub$ = this.machineAreaPnAreasService
      .getAllAreas(new OuterResourcesPnRequestModel())
      .subscribe((data) => {
        if (data && data.success) {
          this.mappingAreas = data.model;
        }
      });
  }

  showEditMachineModal(machine: InnerResourcePnModel) {
    this.editMachineModal.show(machine);
  }

  showDeleteMachineModal(machine: InnerResourcePnModel) {
    this.deleteMachineModal.show(machine);
  }

  showCreateMachineModal() {
    this.createMachineModal.show();
  }

  sortTable(sort: string) {
    this.innerResourcesStateService.onSortTable(sort);
    this.getAllMachines();
  }

  changePage(offset: number) {
    this.innerResourcesStateService.changePage(offset);
    this.getAllMachines();
  }

  onMachineDeleted() {
    this.innerResourcesStateService.onDelete();
    this.getAllMachines();
  }

  onPageSizeChanged(pageSize: number) {
    this.innerResourcesStateService.updatePageSize(pageSize);
    this.getAllMachines();
  }
}
