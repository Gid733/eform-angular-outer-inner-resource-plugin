import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  OuterResourcePnModel,
  OuterResourcesPnModel,
} from '../../../models/area';
import {
  InnerResourcesPnModel,
  InnerResourcesPnRequestModel,
} from '../../../models/machine';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
} from '../../../services';
import { OuterResourcesStateService } from '../state/outer-resources-state-service';
import { TableHeaderElementModel } from 'src/app/common/models';
import { Subscription } from 'rxjs';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';

@AutoUnsubscribe()
@Component({
  selector: 'app-areas-page',
  templateUrl: './outer-resources-page.component.html',
  styleUrls: ['./outer-resources-page.component.scss'],
})
export class OuterResourcesPageComponent implements OnInit, OnDestroy {
  @ViewChild('createAreaModal', { static: false }) createAreaModal;
  @ViewChild('editAreaModal', { static: false }) editAreaModal;
  @ViewChild('deleteAreaModal', { static: false }) deleteAreaModal;
  areasModel: OuterResourcesPnModel = new OuterResourcesPnModel();
  mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();

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

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    public outerResourcesStateService: OuterResourcesStateService
  ) {}

  ngOnInit() {
    this.getAllInitialData();
  }

  ngOnDestroy() {}

  getAllInitialData() {
    this.getAllAreas();
    this.getMachinesForMapping();
  }

  getAllAreas() {
    this.getAllAreasSub$ = this.outerResourcesStateService
      .getAllAreas()
      .subscribe((data) => {
        if (data && data.success) {
          this.areasModel = data.model;
        }
      });
  }

  getMachinesForMapping() {
    this.getAllMachinesSub$ = this.machineAreaPnMachinesService
      .getAllMachines(new InnerResourcesPnRequestModel())
      .subscribe((data) => {
        if (data && data.success) {
          this.mappingMachines = data.model;
        }
      });
  }

  showEditAreaModal(area: OuterResourcePnModel) {
    this.editAreaModal.show(area);
  }

  showDeleteAreaModal(area: OuterResourcePnModel) {
    this.deleteAreaModal.show(area);
  }

  showCreateAreaModal() {
    this.createAreaModal.show();
  }

  sortTable(sort: string) {
    this.outerResourcesStateService.onSortTable(sort);
    this.getAllAreas();
  }

  changePage(offset: number) {
    this.outerResourcesStateService.changePage(offset);
    this.getAllAreas();
  }

  onPageSizeChanged(pageSize: number) {
    this.outerResourcesStateService.updatePageSize(pageSize);
    this.getAllAreas();
  }

  onAreaDeleted() {
    this.outerResourcesStateService.onDelete();
    this.getAllAreas();
  }
}
