import { Component, OnInit } from '@angular/core';
import {ReportPnFullModel, ReportPnGenerateModel} from '../../../models';
import {MachineAreaPnReportsService} from '../../../services';

@Component({
  selector: 'app-machine-area-pn-report-generator',
  templateUrl: './report-generator-container.component.html',
  styleUrls: ['./report-generator-container.component.scss']
})
export class ReportGeneratorContainerComponent implements OnInit {
  reportModel: ReportPnFullModel = new ReportPnFullModel();
  spinnerStatus = false;
  constructor(private reportService: MachineAreaPnReportsService) {}

  ngOnInit() {
  }

  onGenerateReport(model: ReportPnGenerateModel) {
    debugger;
    this.spinnerStatus = true;
    this.reportService.generateReport(model).subscribe((data) => {
      if (data && data.success) {
        this.reportModel = data.model;
      } this.spinnerStatus = false;
    });
  }

  onSaveReport(model: ReportPnGenerateModel) {
    this.spinnerStatus = true;
    this.reportService.getGeneratedReport(model).subscribe((data) => {
      if (data && data.success) {

      } this.spinnerStatus = false;
    });
  }

}
