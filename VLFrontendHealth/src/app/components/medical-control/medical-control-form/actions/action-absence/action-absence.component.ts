import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs/internal/Observable";

import { LoadingSpinnerComponent } from "../../../../loading-spinner/loading-spinner.component";
import { AbsenceType } from "../../../../../models/absence-type.model";
import { AbsenceTypeService } from "../../../../../services/absence-type.service";
import { AlertService } from "../../../../../services/alert.service";
import { Country } from "../../../../../models/country.model";
import { AuthService } from "../../../../../services/auth.service";
import { Select } from "../../../../../models/select.model";
import { MedicalControlsService } from "../../../../../services/medical-controls.service";
import { AbsenceService } from "../../../../../services/absence.service";
import { AbsenceRequest } from "../../../../../models/absence.model";
import { TranslatePipe } from "../../../../../pipes/translate.pipe";
import { EnumTrackingType } from "./../../../../../models/tracking-type.enum";
import { EnumBehaviour } from "./../../../../../models/enum/enum-behaviour.model.ts.enum";

@Component({
	selector: "app-action-absence",
	templateUrl: "./action-absence.component.html",
	styleUrls: ["./action-absence.component.scss"]
})
export class ActionAbsenceComponent implements OnInit, AfterViewInit {
	@ViewChild("absenceSpinner", { static: false })
	public absenceSpinner: LoadingSpinnerComponent;
	public absenceTypesFilter: AbsenceType[];
	public country: Country;
	public pathologies: Select[];
	public formAbsenceMedicalControl: FormGroup;
	public editing = true;
	public form$: Observable<FormGroup>;
	public absenceDateLabel: string;
	public restDaysLabel: string;
	public isFilterAbsenceType = this.medicalControlService.idAbsenceTypeParent;
	public absenceId: number;
	private idTrackingType = this.medicalControlService.idTrackingTypeSelected;
	private sqlMinYear = 1753;
	private _isMedicalControlAccident = false;

	constructor(
		private fb: FormBuilder,
		private absenceTypeService: AbsenceTypeService,
		private alertService: AlertService,
		private authService: AuthService,
		private medicalControlService: MedicalControlsService,
		private absenceService: AbsenceService,
		private route: ActivatedRoute,
		private translatePipe: TranslatePipe
	) {
		this.country = this.authService.getTenantCountry();
		this.formAbsenceMedicalControl = this.fb.group({
			id: [0],
			idAbsenceType: [null, Validators.required],
			idPathology: [0],
			dateFrom: [null, Validators.required],
			dateTo: [null],
			restDays: [null, Validators.required],
			idCountry: [this.country.id]
		});
		this.medicalControlService.actionForm = this.formAbsenceMedicalControl;
		this._isMedicalControlAccident = this.medicalControlService.isMedicalControlAccident;
	}

	ngOnInit(): void {
		this.editing = this.isEditing();
		this.form$ = this.medicalControlService.getActionForm$();
		this.form$.subscribe((form) => {
			this.medicalControlService.actionForm = form;
			if (form.get("id")?.value) {
				this.absenceId = form.get("id").value;
				this.editing = true;
				this.getAbsenceById(form.get("id").value);
				this.getAbsenceTypeByEmployeeAndDate(
					this.medicalControlService.medicalControlData.idEmployee,
					this.formAbsenceMedicalControl.get("dateFrom").value
				);
				this.formAbsenceMedicalControl.get("dateFrom").disable();
			} else {
				this.formAbsenceMedicalControl.get("dateFrom").enable();
				this.editing = false;
			}
		});
		this.medicalControlService.filterAbsenceTypeWhenAbsenceExtension$.subscribe((data) => {
			this.isFilterAbsenceType = data ? data : 0;
			this.formAbsenceMedicalControl.get("idAbsenceType").setValue(null);
			this.subscribeParentAbsenceType();
		});
		this.medicalControlService.filterAbsenceTypeWhenReopening$.subscribe((data) => {
			if (data) {
				this.formAbsenceMedicalControl.get("idAbsenceType").setValue(null);
				this.idTrackingType = data;
				this.subscribeTrackingTypeReopening();
			}
		});
		this.medicalControlService.getAbsenceTypes$.subscribe((employee) => {
			this.getAbsenceTypeByEmployeeAndDate(employee, this.formAbsenceMedicalControl.get("dateFrom").value);
		});
		this.medicalControlService.workAccidentControlType$.subscribe((data) => {
			this._isMedicalControlAccident = data ? data : false;
			if (this.medicalControlService.medicalControlData.idEmployee && this.formAbsenceMedicalControl.get("dateFrom").value) {
				this.getAbsenceTypeByEmployeeAndDate(
					this.medicalControlService.medicalControlData.idEmployee,
					this.formAbsenceMedicalControl.get("dateFrom").value
				);
			}
		});
		this.absenceDateLabel = `${this.translatePipe.transform("absenceDate")}*`;
		this.restDaysLabel = `${this.translatePipe.transform("restDays")}*`;
		this.medicalControlService.onLoadActionAbsence.emit();
	}

	ngAfterViewInit(): void {
		this.isTracking();
	}

	isEditing(): boolean {
		let isFromAbsenceRequest = false;
		this.route.queryParamMap.subscribe((queryParams) => {
			isFromAbsenceRequest = Boolean(queryParams.get("fromAbsenceRequested"));
			if (isFromAbsenceRequest) {
				this.formAbsenceMedicalControl.get("dateFrom").disable();
			}
		});
		return isFromAbsenceRequest;
	}

	isTracking(): void {
		this.route.queryParamMap.subscribe((queryParams) => {
			if (queryParams.get("parent")) {
				this.absenceDateLabel = `${this.translatePipe.transform("certificateDate")}*`;
				this.restDaysLabel = `${this.translatePipe.transform("extendDays")}*`;
			}
		});
	}

	isValidDate(date: Date): boolean {
		return date.getFullYear() >= this.sqlMinYear;
	}

	getAbsenceTypeByEmployeeAndDate(idEmployee: number, date: Date): void {
		if (date && idEmployee) {
			const originalDate = date;
			const dateNew = new Date().At0Time(new Date(originalDate));
			const dateFormatted = new Date(dateNew.format());
			if (this.isValidDate(dateFormatted)) {
				this.absenceTypeService.getByIdEmployeeAndDate(idEmployee, dateFormatted).subscribe(
					(data: AbsenceType[]) => {
						this.medicalControlService.absenceTypes = data.filter((x) => x.occupationalHealth === true);

						if (this.idTrackingType === EnumTrackingType.reopening) {
							this.medicalControlService.absenceTypes = this.medicalControlService.absenceTypes.filter(
								(x) => x.allowReopening === true
							);
						}

						if (!this.editing) {
							this.medicalControlService.absenceTypes = this._isMedicalControlAccident
								? this.medicalControlService.absenceTypes.filter((X) => X.idBehaviour === EnumBehaviour.Accident)
								: this.medicalControlService.absenceTypes.filter((X) => X.idBehaviour !== EnumBehaviour.Accident);
						}
						this.absenceTypesFilter = this.medicalControlService.absenceTypes;
						this.subscribeParentAbsenceType();
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						}
					}
				);
			}
		} else {
			const errors: string[] = [];

			if (!date) {
				errors.push("dateIsEmpty");
			}
			if (!idEmployee) {
				errors.push("employeeIsEmpty");
			}

			this.alertService.error(errors, "medicalControlError");
		}
	}

	subscribeParentAbsenceType(): void {
		if (this.medicalControlService.absenceTypes) {
			this.absenceTypesFilter = this.isFilterAbsenceType
				? this.medicalControlService.absenceTypes.filter((element) => element.id === this.isFilterAbsenceType)
				: this.medicalControlService.absenceTypes;
		}
	}

	subscribeTrackingTypeReopening(): void {
		this.getAbsenceTypeByEmployeeAndDate(
			this.medicalControlService.medicalControlData.idEmployee,
			new Date(this.formAbsenceMedicalControl.get("dateFrom").value)
		);
	}

	getAbsenceById(id: number): void {
		this.absenceService.getById(id).subscribe(
			(data: AbsenceRequest) => {
				this.medicalControlService.isPathology$.next(data.idPathology ? true : false);
				this.formAbsenceMedicalControl.get("idPathology").setValue(data.idPathology);
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
				this.medicalControlService.isPathology$.next(false);
			}
		);
	}

	changeAbsence(id: number): void {
		this.absenceTypeService.getById(id).subscribe(
			(data: any) => {
				this.medicalControlService.isPathology$.next(data.pathology ? true : false);
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	changeFromDateAbsence(date: Date): void {
		this.getAbsenceTypeByEmployeeAndDate(this.medicalControlService.medicalControlData.idEmployee, date);
	}
}
