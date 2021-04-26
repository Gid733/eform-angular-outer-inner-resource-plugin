import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  OuterResourcePnModel,
  OuterResourcePnUpdateModel,
} from '../../../models/area';
import { InnerResourcesPnModel } from '../../../models/machine';
import { OuterInnerResourcePnOuterResourceService } from '../../../services';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-area-edit',
  templateUrl: './outer-resource-edit.component.html',
  styleUrls: ['./outer-resource-edit.component.scss'],
})
export class OuterResourceEditComponent implements OnInit, OnDestroy {
  @ViewChild('frame', { static: false }) frame;
  @Input() mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();
  @Output() onAreaUpdated: EventEmitter<void> = new EventEmitter<void>();
  selectedAreaModel: OuterResourcePnModel = new OuterResourcePnModel();

  updateAreaSub$: Subscription;
  getSingleAreaSub$: Subscription;

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService
  ) {}

  ngOnInit() {}

  ngOnDestroy() {}

  show(areaModel: OuterResourcePnModel) {
    this.getSelectedArea(areaModel.id);
    this.frame.show();
  }

  getSelectedArea(id: number) {
    this.getSingleAreaSub$ = this.machineAreaPnAreasService
      .getSingleArea(id)
      .subscribe((data) => {
        if (data && data.success) {
          this.selectedAreaModel = data.model;
        }
      });
  }

  updateArea() {
    this.updateAreaSub$ = this.machineAreaPnAreasService
      .updateArea(new OuterResourcePnUpdateModel(this.selectedAreaModel))
      .subscribe((data) => {
        if (data && data.success) {
          this.onAreaUpdated.emit();
          this.selectedAreaModel = new OuterResourcePnModel();
          this.frame.hide();
        }
      });
  }

  addToEditMapping(e: any, machineId: number) {
    if (e.target.checked) {
      this.selectedAreaModel.relatedInnerResourcesIds.push(machineId);
    } else {
      this.selectedAreaModel.relatedInnerResourcesIds = this.selectedAreaModel.relatedInnerResourcesIds.filter(
        (x) => x !== machineId
      );
    }
  }

  isChecked(machineId: number) {
    if (
      this.selectedAreaModel.relatedInnerResourcesIds &&
      this.selectedAreaModel.relatedInnerResourcesIds.length > 0
    ) {
      return (
        this.selectedAreaModel.relatedInnerResourcesIds.findIndex(
          (x) => x === machineId
        ) !== -1
      );
    }
    return false;
  }
}
