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
  OuterResourcesPnModel,
  InnerResourcePnCreateModel,
} from '../../../models';
import { OuterInnerResourcePnInnerResourceService } from '../../../services';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machine-create',
  templateUrl: './inner-resource-create.component.html',
  styleUrls: ['./inner-resource-create.component.scss'],
})
export class InnerResourceCreateComponent implements OnInit, OnDestroy {
  @ViewChild('frame', { static: false }) frame;
  @Input() mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel();
  @Output() onMachineCreated: EventEmitter<void> = new EventEmitter<void>();
  checked = false;
  newMachineModel: InnerResourcePnCreateModel = new InnerResourcePnCreateModel();

  createMachineSub$: Subscription;

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService
  ) {}

  ngOnInit() {}

  ngOnDestroy() {}

  show() {
    this.frame.show();
  }

  createMachine() {
    this.createMachineSub$ = this.machineAreaPnMachinesService
      .createMachine(this.newMachineModel)
      .subscribe((data) => {
        if (data && data.success) {
          this.onMachineCreated.emit();
          this.newMachineModel = new InnerResourcePnCreateModel();
          this.frame.hide();
        }
      });
  }

  addToArray(e: any, areaId: number) {
    if (e.target.checked) {
      this.newMachineModel.relatedOuterResourcesIds.push(areaId);
    } else {
      this.newMachineModel.relatedOuterResourcesIds = this.newMachineModel.relatedOuterResourcesIds.filter(
        (x) => x !== areaId
      );
    }
  }

  isChecked(relatedAreaId: number) {
    return (
      this.newMachineModel.relatedOuterResourcesIds.indexOf(relatedAreaId) !==
      -1
    );
  }
}
