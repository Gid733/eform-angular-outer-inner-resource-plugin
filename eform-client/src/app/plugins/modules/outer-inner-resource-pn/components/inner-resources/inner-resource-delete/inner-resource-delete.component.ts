import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { InnerResourcePnModel } from '../../../models';
import { OuterInnerResourcePnInnerResourceService } from '../../../services';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machine-delete',
  templateUrl: './inner-resource-delete.component.html',
  styleUrls: ['./inner-resource-delete.component.scss'],
})
export class InnerResourceDeleteComponent implements OnInit, OnDestroy {
  @ViewChild('frame', { static: false }) frame;
  @Output() onMachineDeleted: EventEmitter<void> = new EventEmitter<void>();
  selectedMachineModel: InnerResourcePnModel = new InnerResourcePnModel();

  deleteMachineSub$: Subscription;

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService
  ) {}

  ngOnInit() {}

  ngOnDestroy() {}

  show(machineModel: InnerResourcePnModel) {
    this.selectedMachineModel = machineModel;
    this.frame.show();
  }

  deleteMachine() {
    this.deleteMachineSub$ = this.machineAreaPnMachinesService
      .deleteMachine(this.selectedMachineModel.id)
      .subscribe((data) => {
        if (data && data.success) {
          this.onMachineDeleted.emit();
          this.selectedMachineModel = new InnerResourcePnModel();
          this.frame.hide();
        }
      });
  }
}
