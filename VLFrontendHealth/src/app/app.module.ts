import { MatTabsModule } from "@angular/material/tabs";
import { ScrollingModule } from "@angular/cdk/scrolling";
import { CommonModule, registerLocaleData } from "@angular/common";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, LOCALE_ID, Inject } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatRippleModule, MatNativeDateModule, DateAdapter, MAT_DATE_LOCALE } from "@angular/material/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatPaginatorIntl, MatPaginatorModule } from "@angular/material/paginator";
import { MatSelectModule } from "@angular/material/select";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatSortModule } from "@angular/material/sort";
import { MatTableModule } from "@angular/material/table";
import { MatTooltipModule } from "@angular/material/tooltip";
import { BrowserModule } from "@angular/platform-browser";
import { MdePopoverModule } from "@material-extended/mde";
import { NgSelectModule } from "@ng-select/ng-select";
import { NgxSpinnerModule } from "ngx-spinner";
import { MatSidenavModule } from "@angular/material/sidenav";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatMenuModule } from "@angular/material/menu";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatRadioModule } from "@angular/material/radio";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatInputModule } from "@angular/material/input";
import { MatMomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from "@angular/material-moment-adapter";
import { MatCardModule } from "@angular/material/card";
import { MatChipsModule } from "@angular/material/chips";
import { NgxMaterialTimepickerModule } from "ngx-material-timepicker";
import localeEsAr from "@angular/common/locales/es-AR";
import * as moment from "moment";
import { NgxFileDropModule } from "ngx-file-drop";
import { NgxDocViewerModule } from "ngx-doc-viewer";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ConfigModule } from "./services/app.config.service";
import { ModalComponent } from "./components/modal/modal.component";
import { LoadingSpinnerComponent } from "./components/loading-spinner/loading-spinner.component";
import { AlertComponent } from "./components/alert/alert.component";
import { BreadCrumbComponent } from "./components/breadcrumb/breadcrumb.component";
import { ButtonComponent } from "./components/button/button.component";
import { FieldValidationDirective } from "./directives/field-validation.directive";
import { FileManagementDirective } from "./directives/file-management.directive";
import { FormValidationDirective } from "./directives/form-validation.directive";
import { DoctorComponent } from "./components/doctor/doctor.component";
import { DoctorsListComponent } from "./components/doctors-list/doctors-list.component";
import { ElipsisGridComponent } from "./components/elipsis-grid/elipsis-grid.component";
import { EmptyRouteComponent } from "./components/empty-route/empty-route.component";
import { HealthDashboardComponent } from "./components/health-dashboard/health-dashboard.component";
import { ImageComponent } from "./components/image/image.component";
import { ExamsComponent } from "./components/medical-history/exams/exams.component";
import { GeneralDataComponent } from "./components/medical-history/general-data/general-data.component";
import { MedicalHistoryComponent } from "./components/medical-history/medical-history.component";
import { SummaryComponent } from "./components/medical-history/summary/summary.component";
import { VaccinesComponent } from "./components/medical-history/vaccines/vaccines.component";
import { MedicalServiceListComponent } from "./components/medical-service-list/medical-service-list.component";
import { MedicalServiceComponent } from "./components/medical-service/medical-service.component";
import { NotesModalComponent } from "./components/notes/notes-modal/notes-modal.component";
import { NotesComponent } from "./components/notes/notes.component";
import { PopoverComponent } from "./components/popover/popover.component";
import { SnackBarComponent } from "./components/snack-bar/snack-bar.component";
import { SuccessComponent } from "./components/success/success.component";
import { VerticalSmallFeedbackComponent } from "./components/vertical-small-feedback/vertical-small-feedback.component";
import { UserPreferenceService } from "./services/user-preference.service";
import { CustomDateAdapter } from "./services/custom-date-adapter.service";
import { UserPreference } from "./models/user-preference.model";
import { MyHttpInterceptor } from "./interceptors/httpInterceptor";
import { AccessResourcePipe } from "./pipes/accessResource.pipe";
import { TranslatePipe } from "./pipes/translate.pipe";
import { ExcelService } from "./services/excel.services";
import { TranslateService } from "./services/translate.service";
import { ClinicalRecordComponent } from "./components/clinical-record/clinical-record.component";
import { VerticalFeedbackComponent } from "./components/vertical-feedback/vertical-feedback.component";
import { CustomMatPaginatorIntl } from "./shared/functions/custom-mat-paginator";
import { SelectEmployeeSearchComponent } from "./components/select-employee-search/select-employee-search.component";
import { ActionAbsenceComponent } from "./components/medical-control/medical-control-form/actions/action-absence/action-absence.component";
import { MedicalControlFormComponent } from "./components/medical-control/medical-control-form/medical-control-form.component";
import { MedicalControlComponent } from "./components/medical-control/medical-control.component";
import { HorizontalFeedbackComponent } from "./components/horizontal-feedback/horizontal-feedback.component";
import { ActionsComponent } from "./components/medical-control/medical-control-form/actions/actions.component";
import { AssetUrlPipe } from "./pipes/asset-url.pipe";
import { ModalExamComponent } from "./components/medical-history/exams/modal-exam/modal-exam.component";
import { ExamUploadComponent } from "./components/medical-history/exams/exam-upload/exam-upload.component";
import { TileViewComponent } from "./components/medical-history/exams/tile-view/tile-view.component";
import { TableViewComponent } from "./components/medical-history/exams/table-view/table-view.component";
import { ModalImagePreviewComponent } from "./components/medical-history/exams/modal-image-preview/modal-image-preview.component";
import { MedicalControlsListComponent } from "./components/medical-controls-grid/medical-controls-list/medical-controls-list.component";
import { MedicalControlsGridComponent } from "./components/medical-controls-grid/medical-controls-grid.component";
import { MedicalControlFilterComponent } from "./components/medical-controls-grid/medical-control-filter/medical-control-filter.component";
import { CertificateUploadComponent } from "./components/certificate-upload/certificate-upload.component";
import { MedicalControlDetailComponent } from "./components/medical-control/medical-control-detail/medical-control-detail.component";
import { ActionBreakComponent } from "./components/medical-control/medical-control-form/actions/action-break/action-break.component";
import { ActionVirtualAttentionComponent } from "./components/medical-control/medical-control-form/actions/action-virtual-attention/action-virtual-attention.component";
import { PathologiesComponent } from "./components/medical-control/medical-control-form/pathologies/pathologies.component";
import { UploadFileComponent } from "./shared/components/upload-file/upload-file.component";
import { DragDropFileComponent } from "./shared/components/drag-drop-file/drag-drop-file.component";
import { ModalFileViewerComponent } from "./shared/components/modal-file-viewer/modal-file-viewer.component";
import { DateFormatPipe } from "./pipes/date-format.pipe";
import { ComplaintComponent } from "./components/medical-control/complaint/complaint.component";
import { WorkElementListComponent } from "./components/medical-control/medical-control-form/work-element-list/work-element-list.component";
import { WorkElementGridComponent } from "./components/medical-control/medical-control-form/work-element-grid/work-element-grid.component";
import { BackgroundImageDirective } from "./directives/background-image.directive";

registerLocaleData(localeEsAr, "es-AR");
@NgModule({
	declarations: [
		AppComponent,
		EmptyRouteComponent,
		TranslatePipe,
		AccessResourcePipe,
		HealthDashboardComponent,
		BreadCrumbComponent,
		ClinicalRecordComponent,
		LoadingSpinnerComponent,
		AlertComponent,
		ElipsisGridComponent,
		ModalComponent,
		ElipsisGridComponent,
		AlertComponent,
		DoctorsListComponent,
		SnackBarComponent,
		DoctorComponent,
		ButtonComponent,
		SuccessComponent,
		MedicalServiceComponent,
		MedicalServiceListComponent,
		MedicalHistoryComponent,
		SummaryComponent,
		GeneralDataComponent,
		ExamsComponent,
		VaccinesComponent,
		ImageComponent,
		ButtonComponent,
		SnackBarComponent,
		FieldValidationDirective,
		FileManagementDirective,
		FormValidationDirective,
		AlertComponent,
		NotesComponent,
		NotesModalComponent,
		PopoverComponent,
		VerticalSmallFeedbackComponent,
		HorizontalFeedbackComponent,
		ModalExamComponent,
		ExamUploadComponent,
		TileViewComponent,
		TableViewComponent,
		VerticalFeedbackComponent,
		MedicalControlFormComponent,
		SelectEmployeeSearchComponent,
		ActionAbsenceComponent,
		MedicalControlComponent,
		HorizontalFeedbackComponent,
		VerticalFeedbackComponent,
		ActionsComponent,
		AssetUrlPipe,
		ModalImagePreviewComponent,
		MedicalControlsListComponent,
		VerticalFeedbackComponent,
		MedicalControlsGridComponent,
		MedicalControlFilterComponent,
		MedicalControlComponent,
		HorizontalFeedbackComponent,
		CertificateUploadComponent,
		MedicalControlDetailComponent,
		ActionBreakComponent,
		ActionVirtualAttentionComponent,
		PathologiesComponent,
		UploadFileComponent,
		DragDropFileComponent,
		ModalFileViewerComponent,
		DateFormatPipe,
		ComplaintComponent,
		WorkElementGridComponent,
		WorkElementListComponent,
		BackgroundImageDirective
	],
	imports: [
		MdePopoverModule,
		BrowserModule,
		AppRoutingModule,
		CommonModule,
		FormsModule,
		MatDialogModule,
		MatSnackBarModule,
		ReactiveFormsModule,
		HttpClientModule,
		MatSelectModule,
		NgxSpinnerModule,
		MatTooltipModule,
		MatSortModule,
		MatButtonModule,
		MatRippleModule,
		MatTableModule,
		MatTableModule,
		MatSortModule,
		MatPaginatorModule,
		MatTooltipModule,
		MatNativeDateModule,
		MatSidenavModule,
		BrowserAnimationsModule,
		MatDatepickerModule,
		MatChipsModule,
		MatMenuModule,
		MatExpansionModule,
		MatRadioModule,
		MatAutocompleteModule,
		MatInputModule,
		MatCardModule,
		NgSelectModule,
		MatFormFieldModule,
		NgxSpinnerModule,
		HttpClientModule,
		MatChipsModule,
		MatTableModule,
		MatSortModule,
		MatPaginatorModule,
		MatMenuModule,
		MatSortModule,
		MatMomentDateModule,
		MatTabsModule,
		NgxMaterialTimepickerModule,
		NgxFileDropModule,
		NgxDocViewerModule,
		MatSlideToggleModule
	],
	exports: [
		TranslatePipe,
		AccessResourcePipe,
		ScrollingModule,
		MatFormFieldModule,
		MatPaginatorModule,
		MatSortModule,
		BackgroundImageDirective
	],
	providers: [
		TranslatePipe,
		DateFormatPipe,
		TranslateService,
		ConfigModule.init(),
		AccessResourcePipe,
		{ provide: HTTP_INTERCEPTORS, useClass: MyHttpInterceptor, multi: true },
		{ provide: LOCALE_ID, useValue: "es-AR" },
		{ provide: MAT_DATE_LOCALE, useValue: "es" },
		{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: false } },
		{
			provide: DateAdapter,
			useClass: CustomDateAdapter,
			deps: [UserPreferenceService]
		},
		UserPreference,
		ExcelService,
		{ provide: MatPaginatorIntl, useClass: CustomMatPaginatorIntl }
	],
	entryComponents: [ModalComponent, ModalExamComponent, ModalImagePreviewComponent, ModalFileViewerComponent],
	schemas: [CUSTOM_ELEMENTS_SCHEMA],
	bootstrap: [AppComponent]
})
export class AppModule {
	constructor(@Inject(LOCALE_ID) locale: string) {
		registerLocaleData(localeEsAr, "es-AR");
		moment.locale(locale);
	}
}
