import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { AuthGuardService } from "./services/auth-guard-service.service";
import { EmptyRouteComponent } from "./components/empty-route/empty-route.component";
import { HealthDashboardComponent } from "./components/health-dashboard/health-dashboard.component";
import { ClinicalRecordComponent } from "./components/clinical-record/clinical-record.component";
import { DoctorComponent } from "./components/doctor/doctor.component";
import { DoctorsListComponent } from "./components/doctors-list/doctors-list.component";
import { MedicalHistoryComponent } from "./components/medical-history/medical-history.component";
import { MedicalServiceListComponent } from "./components/medical-service-list/medical-service-list.component";
import { MedicalServiceComponent } from "./components/medical-service/medical-service.component";
import { SuccessComponent } from "./components/success/success.component";
import { MedicalControlFormComponent } from "./components/medical-control/medical-control-form/medical-control-form.component";
import { MedicalControlsGridComponent } from "./components/medical-controls-grid/medical-controls-grid.component";
import { MedicalControlComponent } from "./components/medical-control/medical-control.component";
import { ActionAbsenceComponent } from "./components/medical-control/medical-control-form/actions/action-absence/action-absence.component";
import { MedicalControlDetailComponent } from "./components/medical-control/medical-control-detail/medical-control-detail.component";
import { ActionBreakComponent } from "./components/medical-control/medical-control-form/actions/action-break/action-break.component";
import { ActionVirtualAttentionComponent } from "./components/medical-control/medical-control-form/actions/action-virtual-attention/action-virtual-attention.component";
import { UploadFileComponent } from "./shared/components/upload-file/upload-file.component";
import { ComplaintComponent } from "./components/medical-control/complaint/complaint.component";

const childrenOperationsActions = [
	{
		path: "action/absence",
		component: ActionAbsenceComponent,
		canActivate: [AuthGuardService]
	},
	{
		path: "action/break",
		component: ActionBreakComponent,
		canActivate: [AuthGuardService]
	},
	{
		path: "action/virtual-attention",
		component: ActionVirtualAttentionComponent,
		canActivate: [AuthGuardService]
	}
];

const routes: Routes = [
	{
		path: "healthApp",
		component: HealthDashboardComponent
	},
	{
		path: "healthApp/doctors",
		component: DoctorsListComponent,
		canActivate: [AuthGuardService],
		data: { resource: "Doctors" }
	},
	{
		path: "healthApp/clinical-records",
		component: ClinicalRecordComponent,
		canActivate: [AuthGuardService],
		data: { resource: "ClinicalRecords" }
	},
	{
		path: "healthApp/clinical-records/:id/complaint",
		component: ComplaintComponent,
		canActivate: [AuthGuardService],
		data: { resource: "ClinicalRecordsComplaint" }
	},
	{
		path: "healthApp/doctors/create",
		component: DoctorComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthDoctorCreate" }
	},
	{
		path: "healthApp/doctors/modify/:id",
		component: DoctorComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthDoctorEdit" }
	},
	{
		path: "healthApp/medical-services",
		component: MedicalServiceListComponent
	},
	{
		path: "healthApp/upload-file",
		component: UploadFileComponent
	},
	{
		path: "healthApp/medical-service/create",
		component: MedicalServiceComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalServiceCreate" }
	},
	{
		path: "healthApp/medical-service/modify/:id",
		component: MedicalServiceComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalServiceEdit" }
	},
	{
		path: "healthApp/success",
		component: SuccessComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthSuccess" }
	},
	{
		path: "healthApp/medical-history/modify/:id",
		component: MedicalHistoryComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalServiceEdit" }
	},
	{
		path: "healthApp/medical-controls",
		component: MedicalControlsGridComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalControls" }
	},
	{
		path: "healthApp/medical-control",
		component: MedicalControlComponent,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalControlCreate" }
	},
	{
		path: "healthApp/medical-control/create",
		component: MedicalControlFormComponent,
		children: childrenOperationsActions,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalControlCreate" }
	},
	{
		path: "healthApp/medical-control/:id",
		component: MedicalControlFormComponent,
		children: childrenOperationsActions,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalControlEdit" }
	},
	{
		path: "healthApp/medical-control/detail/:id",
		component: MedicalControlDetailComponent,
		children: childrenOperationsActions,
		canActivate: [AuthGuardService],
		data: { resource: "HealthMedicalControlDetail" }
	},
	{
		path: "healthApp/medical-control/:id/complaint",
		component: ComplaintComponent
		// canActivate: [AuthGuardService],
		// data: { resource: "ClinicalRecordsComplaint" }
	},
	{
		path: "**",
		component: EmptyRouteComponent
	}
];

@NgModule({
	imports: [RouterModule.forRoot(routes)],
	exports: [RouterModule]
})
export class AppRoutingModule {}
