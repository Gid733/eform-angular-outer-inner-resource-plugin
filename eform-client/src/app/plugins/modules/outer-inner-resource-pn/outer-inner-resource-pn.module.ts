import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { NgSelectModule } from '@ng-select/ng-select';

import {
  OuterInnerResourcePnOuterResourceService,
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnReportsService,
  OuterInnerResourcePnSettingsService,
} from './services';
import { OuterInnerResourcePnLayoutComponent } from './layouts';
import { SharedPnModule } from '../shared/shared-pn.module';
import {
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourcesPageComponent,
  OuterResourceEditComponent,
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourcesPageComponent,
  InnerResourceEditComponent,
  ReportPreviewTableComponent,
  ReportGeneratorFormComponent,
  OuterInnerResourceSettingsComponent,
  ReportGeneratorContainerComponent,
  InnerResourcesStateService,
  OuterResourcesStateService,
} from './components';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { OuterInnerResourcePnRouting } from './outer-inner-resource-pn-routing.module';
import { OwlDateTimeModule } from 'ng-pick-datetime-ex';
import { EformSharedModule } from 'src/app/common/modules/eform-shared/eform-shared.module';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    OuterInnerResourcePnRouting,
    TranslateModule,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    OwlDateTimeModule,
    EformSharedModule,
  ],
  declarations: [
    OuterResourcesPageComponent,
    OuterResourceDeleteComponent,
    OuterResourceCreateComponent,
    OuterResourceEditComponent,
    InnerResourcesPageComponent,
    InnerResourceCreateComponent,
    InnerResourceEditComponent,
    InnerResourceDeleteComponent,
    OuterInnerResourcePnLayoutComponent,
    OuterInnerResourceSettingsComponent,
    ReportGeneratorContainerComponent,
    ReportGeneratorFormComponent,
    ReportPreviewTableComponent,
  ],
  providers: [
    OuterInnerResourcePnOuterResourceService,
    OuterInnerResourcePnInnerResourceService,
    OuterInnerResourcePnSettingsService,
    OuterInnerResourcePnReportsService,
    InnerResourcesStateService,
    OuterResourcesStateService,
  ],
})
export class OuterInnerResourcePnModule {}
