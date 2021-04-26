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
  OuterResourcePnCreateModel,
  InnerResourcesPnModel,
} from '../../../models';
import { OuterInnerResourcePnOuterResourceService } from '../../../services';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-area-create',
  templateUrl: './outer-resource-create.component.html',
  styleUrls: ['./outer-resource-create.component.scss'],
})
export class OuterResourceCreateComponent implements OnInit, OnDestroy {
  @ViewChild('frame', { static: false }) frame;
  @Input() mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();
  @Output() onAreaCreated: EventEmitter<void> = new EventEmitter<void>();
  newAreaModel: OuterResourcePnCreateModel = new OuterResourcePnCreateModel();

  createAreaSub$: Subscription;

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService
  ) {}

  ngOnInit() {}

  ngOnDestroy() {}

  show() {
    this.frame.show();
  }

  createArea() {
    this.createAreaSub$ = this.machineAreaPnAreasService
      .createArea(this.newAreaModel)
      .subscribe((data) => {
        if (data && data.success) {
          this.onAreaCreated.emit();
          this.newAreaModel = new OuterResourcePnCreateModel();
          this.frame.hide();
        }
      });
  }

  addToArray(e: any, machineId: number) {
    if (e.target.checked) {
      this.newAreaModel.relatedInnerResourcesIds.push(machineId);
    } else {
      this.newAreaModel.relatedInnerResourcesIds = this.newAreaModel.relatedInnerResourcesIds.filter(
        (x) => x !== machineId
      );
    }
  }

  isChecked(relatedMachineId: number) {
    return (
      this.newAreaModel.relatedInnerResourcesIds.indexOf(relatedMachineId) !==
      -1
    );
  }
}
