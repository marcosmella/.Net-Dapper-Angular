import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AfterViewInit, Component, OnInit, ViewChild, OnDestroy } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";

import { TranslatePipe } from "../../../pipes/translate.pipe";
import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
import { MedicalControlTypeService } from "../../../services/medical-control-type.service";
import { Select } from "../../../models/select.model";
import { AlertService } from "../../../services/alert.service";
import { MedicalServiceService } from "../../../services/medical-service.service";
import { MedicalService } from "../../../models/medical-service";
import { MedicalControlActionService } from "../../../services/medical-control-action.service";
import { DoctorService } from "../../../services/doctor.service";
import { Doctor } from "../../../models/doctors.model";
import { PersonService } from "../../../services/person.service";
import { PersonalInformation } from "../../../models/personal-information.model";
import { MedicalControlsService } from "./../../../services/medical-controls.service";
import {
	MedicalControl,
	MedicalControlChild,
	MedicalControlList,
	MedicalControlsFilterParameters,
	MedicalControlTrackingDate
} from "../../../models/medical-control.model";
import { SearchEngineFilterService } from "../../../services/search-engine-filter.service";
import { AbsenceFilterParameters, DateRange, FilterEmployee } from "../../../models/employee-filter.model";
import { FilterAbsence } from "./../../../models/absence-filter.model";
import { AbsenceRange } from "./../../../models/absence-range.model";
import { UserPreference } from "../../../models/user-preference.model";
import { AbsenceTypeService } from "../../../services/absence-type.service";
import { AbsenceService } from "../../../services/absence.service";
import { AbsenceType } from "./../../../models/absence-type.model";
import { Absence, AbsenceRequest } from "./../../../models/absence.model";
import { SuccessService } from "../../../services/success.service";
import { MedicalControlAbsence } from "./../../../models/medical-control-absence.model";
import { EnumFeedback } from "../../../models/enumFeedback.model";
import { EnumMedicalControlActions } from "../../../models/medicalControlAction.enum";
import { UserPreferenceService } from "./../../../services/user-preference.service";
import { TrackingType } from "../../../models/tracking-type.model";
import { MedicalControlTrackingTypesService } from "./../../../services/medical-control-tracking-types.service";
import { MedicalControlRow } from "./../../../models/medical-control.model";
import { Pathologies } from "../../../models/pathology.model";
import { DateValidators } from "./../../../validators/date-validator";
import { SnackBarService } from "./../../../services/snack-bar.service";
import { PathologyService } from "../../../services/pathology.service";
import { EnumTrackingType } from "./../../../models/tracking-type.enum";
import { DragDropFileComponent } from "./../../../shared/components/drag-drop-file/drag-drop-file.component";
import { ElipsisAction, IconTypes } from "../../elipsis-grid/elipsis-grid.component";
import { DragDropFileService } from "../../../services/drag-drop-file.service";
import { UploadFileParameter } from "../../../models/upload-file-parameter.model";
import { EnumControlType } from "./../../../models/enum/control-type.model";
import { MenuService } from "../../../services/menu.service";
import { TdTrackingService } from "./../../../services/TD/td-tracking.service";
import { AbsenceStatusEnum } from "./../../../models/enum/absence-status.enum";
import { EnumBehaviour } from "./../../../models/enum/enum-behaviour.model.ts.enum";
import { Country } from "../../../models/country.model";
import { AuthService } from "../../../services/auth.service";
import { ModalService } from "../../../services/modal.service";
@Component({
	selector: "app-medical-control-form",
	templateUrl: "./medical-control-form.component.html",
	styleUrls: ["./medical-control-form.component.scss"]
})
export class MedicalControlFormComponent implements OnInit, AfterViewInit, OnDestroy {
	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;
	public formMedicalControl: FormGroup;
	public editing = false;
	public fullName: string;
	public medicalControlTypes: Select[];
	public medicalService: MedicalService[];
	public doctors: Doctor[];
	public doctor = true;
	public medicalControlActions: Select[];
	public fileNumber: string;
	public action: number;
	public absenceSearch: Array<FilterAbsence>;
	public absencesTypeList: Select[];
	public absencesRangeList: AbsenceRange[];
	public absencesTypeRangeList: AbsenceRange[];
	public isAbsence = false;
	public fromAbsenceRequested = false;
	public idPerson: number;
	public idCertificate = 0;
	public absenceAssignedToMedicalControl: number[];
	public actionAndTypeEditing = true;
	public minSelectDate = new Date("1900-01-01");
	public maxSelectDate: Date;
	public timeFormat = this.userPreferenceService.getTimeFormatApp();
	public defaultTime = moment(new Date()).format("HH:mm");
	public isTracking: boolean;
	public idParent: number;
	public medicalControlFormDescription = this.translatePipe.transform("medicalControlCharge");
	public trackingTypes: TrackingType[];
	public queryParams: ActivatedRoute;
	public enumAbsence = [
		EnumMedicalControlActions.absence,
		EnumMedicalControlActions.inItinere,
		EnumMedicalControlActions.professionalIllness,
		EnumMedicalControlActions.workAccident
	];
	public idMedicalControl: number;
	public pathologies = new Array<Pathologies>();
	public viewPathologies = false;
	public absenceParentId: number;
	public trackingDate = new MedicalControlTrackingDate();
	public medicalControlData: MedicalControl;
	public medicalControlWorkAccident: boolean;
	public hiddenWorkElementGrid = true;
	public acceptFormats: string[] = [".png", ".jpg", ".pdf", ".jpeg"];
	@ViewChild("dragAndDropFile", { static: false })
	public dragAndDropFile: DragDropFileComponent;
	public ellipsisActions = new Array<ElipsisAction>();
	public idFileType = 10003;
	public entityTypeIdEmployee = 1;
	public active = false;
	public updateData: any;
	public fileType = "file";
	public idControlType: EnumControlType;
	public viewToggleAbsenceRequested = false;
	public deletePathology: number;
	public country: Country;
	public showEmployeeSearch = true;
	public idAbsence = 0;
	protected dateValidators: DateValidators;
	private personalInformation: PersonalInformation;
	private fromMedicalHistory = false;
	private allMedicalControlTypes: Select[];
	private pathologiesChange = false;

	constructor(
		private translatePipe: TranslatePipe,
		private fb: FormBuilder,
		private alertService: AlertService,
		private medicalControlTypeService: MedicalControlTypeService,
		private medicalServiceService: MedicalServiceService,
		private doctorService: DoctorService,
		private medicalControlActionService: MedicalControlActionService,
		private personService: PersonService,
		public medicalControlService: MedicalControlsService,
		private route: ActivatedRoute,
		private searchEngineFilterService: SearchEngineFilterService,
		private absenceService: AbsenceService,
		private absenceTypeService: AbsenceTypeService,
		private userPreference: UserPreference,
		private router: Router,
		private successServ: SuccessService,
		private userPreferenceService: UserPreferenceService,
		private snackBarService: SnackBarService,
		private medicalControlTrackingTypesService: MedicalControlTrackingTypesService,
		private patholgyService: PathologyService,
		private dragDropFileService: DragDropFileService,
		private menuService: MenuService,
		private tdTrackingService: TdTrackingService,
		private modalService: ModalService,
		private authService: AuthService
	) {
		this.createForm();
	}

	ngOnInit(): void {
		this.menuService.hideMenu();

		this.getDoctors();
		this.getMedicalControlType();
		this.getMedicalService();
		this.setEllipsis();
	}

	ngAfterViewInit(): void {
		this.getParams();
	}

	ngOnDestroy(): void {
		this.menuService.showMenu();
	}

	createForm(): void {
		this.medicalControlService.medicalControlData = new MedicalControl();
		this.medicalControlService.medicalControlData.absence = new MedicalControlAbsence();
		this.formMedicalControl = this.fb.group({
			id: [0],
			idEmployee: [null, [Validators.required]],
			date: [null, [Validators.required]],
			idControlType: [null, [Validators.required]],
			idAction: [null, [Validators.required]],
			idDoctor: [null],
			idMedicalService: [null, [Validators.required]],
			idOccupationalDoctor: [null, [Validators.required]],
			doctor: [true],
			privateDoctorName: [null],
			enrollment: [null, [Validators.required, Validators.minLength(0), Validators.maxLength(20)]],
			diagnosis: [null, [Validators.minLength(0), Validators.maxLength(250)]],
			idAbsenceType: [null],
			idAbsence: [null],
			timeControl: [null],
			idFile: [0],
			idParent: [null],
			idTrackingType: [null],
			dateFrom: [null],
			dateTo: [null],
			pathologies: [0],
			filename: [null],
			toggleAbsenceRequested: [null]
		});
		this.formMedicalControl.get("enrollment").disable();
		this.medicalControlService.isPathology$.subscribe((data) => {
			this.viewPathologies = data;
		});
		this.medicalControlData = new MedicalControl();
		this.country = this.authService.getTenantCountry();
	}

	setDateRangeValidators(): void {
		this.formMedicalControl.get("date").valueChanges.subscribe((date) => {
			if (date) {
				this.validateDateRange();
			}
		});
		this.medicalControlService.actionForm.get("dateFrom").valueChanges.subscribe((dateFrom) => {
			if (dateFrom) {
				this.validateDateRange();
			}
		});
		this.medicalControlService.actionForm.get("restDays").valueChanges.subscribe((days) => {
			if (days) {
				this.validateDateRange();
			}
		});

		this.formMedicalControl.get("date").updateValueAndValidity();
	}

	validateDateRange(): void {
		const dateFrom = this.medicalControlService.actionForm.get("dateFrom").value;
		const days = this.medicalControlService.actionForm.get("restDays").value;

		if (this.generateAbsence(this.action) && dateFrom && days) {
			const dateTo = moment(this.medicalControlService.actionForm.get("dateFrom").value).add(days - 1, "days");
			const date = this.formMedicalControl.get("date").value;
			if (date < dateFrom || date > dateTo) {
				this.formMedicalControl.get("date").setErrors({ InvalidDateRanges: true });
			} else {
				this.formMedicalControl.get("date").setErrors(null);
			}
		}
	}

	unsetDateRangeValidators(): void {
		this.formMedicalControl.get("date").setErrors({ InvalidDateRanges: null });
		this.formMedicalControl.get("date").updateValueAndValidity();
	}

	getParams(): void {
		this.route.queryParamMap.subscribe((queryParams) => {
			this.fromAbsenceRequested = Boolean(queryParams.get("fromAbsenceRequested"));
			this.idParent = Number(queryParams.get("parent"));
			this.idAbsence = Number(queryParams.get("idAbsence"));
			this.newMedicalControlFromAbsence(this.idAbsence);
			this.queryParams = this.route;
			this.getTrackingTypes();

			if (Number(queryParams.get("medicalHistoryIdEmployee"))) {
				this.idPerson = Number(queryParams.get("medicalHistoryIdEmployee"));
				this.fromMedicalHistory = true;
				this.newMedicalControlFromMedicalHistory();
			}
		});
		this.route.paramMap.subscribe((params) => {
			this.idMedicalControl = Number(params.get("id"));
			if (this.idMedicalControl) {
				this.getMedicalControl(this.idMedicalControl);
			} else {
				this.getTracking();
			}
		});
	}

	newMedicalControlFromMedicalHistory(): void {
		const fileNumber = true;

		this.getPerson(this.idPerson).then(() => {
			this.showEmployeeSearch = false;
			this.getEmployeeData(this.idPerson, fileNumber);
			if (this.fromAbsenceRequested) {
				this.getAbsences();
				this.isAbsence = true;
			}
			const actionGenerateAbsence = this.enumAbsence.find((x) => x === this.action);
			if (actionGenerateAbsence) {
				this.getAbsenceTypes();
			}
		});
	}

	newMedicalControlFromAbsence(id: number): void {
		if (id) {
			this.editing = true;
			this.showEmployeeSearch = false;
			this.actionAndTypeEditing = false;
			this.isAbsence = true;
			this.getAbsenceById(id).then((data) => {
				const newMedicalControlFromAbsence = true;
				this.setAbsenceData(data, newMedicalControlFromAbsence);
			});
			this.formMedicalControl.get("idEmployee").setValidators(null);
		}
	}

	getTracking(): void {
		if (this.idParent) {
			this.spinner.show();
			this.medicalControlFormDescription = this.translatePipe.transform("addTracking");
			this.formMedicalControl.get("idParent").setValue(this.idParent);
			const tracking = true;

			this.getMedicalControlData(this.idParent, tracking)
				.then((data: any) => {
					if (!this.formMedicalControl.get("idEmployee").value) {
						const fileNumber = false;
						this.getEmployeeData(data.idEmployee, fileNumber);
					}
					this.medicalControlService.idControlTypeParent = data.idControlType;

					const lastAbsenceId = data.tracking.map((x) => x.idAbsence).sort((a, b) => a - b)[data.tracking.length - 1];
					const trackingAbsenceId = lastAbsenceId ? lastAbsenceId : data.idAbsence;
					this.absenceParentId = trackingAbsenceId;
					this.pathologies = this.setTrackingPathologies(data);
					this.setTrackingAndValidators(data);
					if (data.idTrackingType) {
						this.sendTrackingType(data.idTrackingType);
						this.hideWorkElementGrid(data.idControlType);
					}
				})
				.catch((error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				})
				.finally(() => {
					this.spinner.hide();
				});
		}
	}

	setTrackingAndValidators(data: MedicalControl): void {
		this.medicalControlData = data;
		this.idPerson = data.idEmployee;
		this.formMedicalControl.get("idEmployee").setValue(data.idEmployee);
		this.formMedicalControl.get("dateFrom").setValue(data.date);
		this.trackingDate.lastMedicalControlDate = data.tracking[0] ? data.tracking[data.tracking.length - 1].date : data.date;
		const nullTrackingType = 0;
		this.trackingDate.lastMedicalControlTrackingType = data.tracking[0]
			? data.tracking[data.tracking.length - 1].idTrackingType
			: nullTrackingType;
		const lastAbsenceId = data.tracking.map((x) => x.idAbsence).sort((a, b) => a - b)[data.tracking.length - 1];
		const trackingAbsenceId = lastAbsenceId ? lastAbsenceId : this.absenceParentId;
		if (data.pathologies && data.pathologies.length === 0) {
			this.viewPathologies = false;
		}
		this.medicalControlData.absence = new MedicalControlAbsence();
		this.medicalControlData.absence.id = this.absenceParentId;
		if (trackingAbsenceId) {
			this.getAbsenceById(trackingAbsenceId).then((absence) => {
				this.trackingDate.absenceDateFrom = absence.dateFrom;
				const absenceDateTo = new Date(absence.dateTo);
				absenceDateTo.setDate(absenceDateTo.getDate() + 1);
				this.trackingDate.absenceDateTo = absenceDateTo;
				const revision = false;
				this.setMinAndMaxDateControl(revision);
				this.formMedicalControl.get("dateTo").setValue(absence.dateTo);
				this.filterTrackingType();
			});
		} else {
			this.trackingDate = null;
			this.filterTrackingType();
		}

		this.formMedicalControl.get("dateFrom").disable();
		this.formMedicalControl.get("dateTo").disable();
		this.formMedicalControl.get("idAction").setValidators(null);
	}

	filterTrackingType(): void {
		const trackingTypeAvailable = this.tdTrackingService.enableTrackingTypes(this.medicalControlData);

		if (this.trackingTypes) {
			this.trackingTypes = this.trackingTypes.filter((x) => trackingTypeAvailable?.some((y) => y === x.id));
		}
	}

	setMinAndMaxDateControl(isRevision: boolean): void {
		const defaultMaxDate = new Date("2099-12-31");
		const minDate = this.idMedicalControl ? this.trackingDate.lastMedicalControlDate : this.trackingDate?.absenceDateTo;
		this.minSelectDate = isRevision ? this.trackingDate.lastMedicalControlDate : minDate;
		this.maxSelectDate =
			isRevision && this.trackingDate.lastMedicalControlTrackingType !== EnumTrackingType.patientReleaseDate
				? this.formMedicalControl.get("dateTo").value
				: defaultMaxDate;
		this.formMedicalControl.get("date").setValue(this.minSelectDate);
	}

	setControlPathologies(data: MedicalControl, isModify: boolean): Array<Pathologies> {
		this.viewPathologies = true;
		const pathologies = new Array<Pathologies>();
		data.pathologies.forEach((x) => {
			x.isModify = isModify;
			x.allowSaveInMedicalControl = isModify;
			pathologies.push(x);
		});
		return pathologies;
	}

	orderTracking(data: MedicalControl): Array<MedicalControlChild> {
		return data.tracking.sort((a, b) => {
			return a.id - b.id;
		});
	}

	setTrackingPathologies(data: MedicalControl): Array<Pathologies> {
		let pathologies = new Array<Pathologies>();
		if (this.idParent) {
			const modify = false;
			pathologies = this.setControlPathologies(data, modify);
		}
		const orderTracking = this.orderTracking(data);
		orderTracking.forEach((x) => {
			if (x.id <= this.idMedicalControl || this.idMedicalControl === 0) {
				const isModify = x.id === this.idMedicalControl ? true : false;
				x.pathologies.forEach((element) => {
					element.isModify = isModify;
					element.allowSaveInMedicalControl = isModify;
					pathologies.push(element);
				});
			}
		});
		return pathologies;
	}

	getMedicalControlType(): void {
		this.medicalControlTypeService.get().subscribe(
			(data: Select[]) => {
				this.allMedicalControlTypes = data;
				this.medicalControlTypes = this.idParent ? this.filterMedicalControlTrackingTypes() : this.filterMedicalControlTypes();
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	filterMedicalControlTrackingTypes(): Select[] {
		return this.allMedicalControlTypes.filter((x) => x.id !== EnumControlType.WorkAccident && x.id !== EnumControlType.AccidentComplaint);
	}

	filterMedicalControlTypes(idBehaviour: number = 0): Select[] {
		let filterControlTypes = this.allMedicalControlTypes;
		if (this.fromAbsenceRequested) {
			filterControlTypes = filterControlTypes.filter((x) => x.id !== EnumControlType.AccidentComplaint);
		}
		if (idBehaviour) {
			filterControlTypes =
				idBehaviour === EnumBehaviour.Accident
					? filterControlTypes.filter((x) => x.id === EnumControlType.WorkAccident)
					: filterControlTypes.filter((x) => x.id !== EnumControlType.WorkAccident);
		}
		return filterControlTypes;
	}

	getMedicalService(): void {
		this.medicalServiceService.get().subscribe(
			(data: MedicalService[]) => {
				this.medicalService = data;
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	getDoctors(): void {
		this.doctorService.get().subscribe(
			(data: Doctor[]) => {
				this.doctors = data.map((x) => ({
					id: x.id,
					fullName: `${x.lastName}, ${x.firstName}`,
					firstName: x.firstName,
					lastName: x.lastName,
					enrollment: x.enrollment,
					enrollmentExpirationDate: x.enrollmentExpirationDate,
					documentNumber: x.documentNumber,
					documentExpirationDate: x.documentExpirationDate
				}));
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	getTrackingTypes(): void {
		if (this.idParent) {
			this.medicalControlTrackingTypesService.get().subscribe(
				(data: TrackingType[]) => {
					this.trackingTypes = data;
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			);
		}
	}

	trackingTypeChange(id: number): void {
		const trackingTypes = this.trackingTypes.find((x) => x.id === id);
		this.medicalControlService.idTrackingTypeSelected = id;
		this.action = trackingTypes.createAbsence ? EnumMedicalControlActions.absence : 0;
		this.isAbsence = false;
		const formAction = this.action ? this.action : EnumMedicalControlActions.examination;
		this.formMedicalControl.get("idAction").setValue(formAction);

		this.medicalControlService.medicalControlData.idControlType = id;
		this.setAbsenceType(id);
		const isRevision = EnumTrackingType.revision === id ? true : false;
		this.setMinAndMaxDateControl(isRevision);
		this.sendTrackingType(id);
		this.isViewToggleFromAbsenceRequested(id);
		this.absenceSearch = undefined;
		if (!this.fileNumber) {
			this.getPerson(this.idPerson);
		}
	}

	isViewToggleFromAbsenceRequested(idTrackingType: number): void {
		this.viewToggleAbsenceRequested =
			idTrackingType === EnumTrackingType.absenceExtension ||
			idTrackingType === EnumTrackingType.reopening ||
			idTrackingType === EnumTrackingType.absenceNotEndorsed ||
			idTrackingType === EnumTrackingType.generationAbsence
				? true
				: false;
		this.medicalControlService.medicalControlData.absence.id = 0;
		this.formMedicalControl.get("toggleAbsenceRequested").setValue(false);
	}

	setAbsenceType(id: number): void {
		if (id === EnumTrackingType.absenceExtension || id === EnumTrackingType.reopening) {
			this.getAbsenceById(this.absenceParentId).then((data) => {
				this.medicalControlData.absence.idAbsenceType = data.idAbsenceType;
				if (id === EnumTrackingType.absenceExtension) {
					this.medicalControlService.idAbsenceTypeParent = data.idAbsenceType;
					this.medicalControlService.filterAbsenceTypeWhenAbsenceExtension$.next(data.idAbsenceType);
				}
			});
		} else {
			this.medicalControlService.filterAbsenceTypeWhenAbsenceExtension$.next(null);
			this.medicalControlData.absence.idAbsenceType = null;
		}
	}

	sendTrackingType(idTrackingType: number): void {
		this.medicalControlService.isMedicalControlAccident =
			this.medicalControlService.idControlTypeParent === EnumControlType.WorkAccident ||
			this.medicalControlService.idControlTypeParent === EnumControlType.AccidentComplaint;

		if (
			idTrackingType === EnumTrackingType.reopening ||
			idTrackingType === EnumTrackingType.absenceExtension ||
			idTrackingType === EnumTrackingType.generationAbsence
		) {
			this.medicalControlService.filterAbsenceTypeWhenReopening$.next(idTrackingType);
			this.medicalControlService.workAccidentControlType$.next(this.medicalControlService.isMedicalControlAccident);
		} else {
			this.medicalControlService.isMedicalControlAccident = false;
			this.medicalControlService.filterAbsenceTypeWhenReopening$.next(null);
			this.medicalControlService.workAccidentControlType$.next(false);
		}
	}

	getMedicalControlData(id: number, tracking: boolean = false): Promise<any> {
		return new Promise((resolve, reject) => {
			this.spinner.show();
			this.medicalControlService
				.get(id, tracking)
				.subscribe(
					(data: MedicalControl | MedicalControlRow) => {
						resolve(data);
					},
					(error) => {
						reject(error);
					}
				)
				.add(() => {
					this.spinner.hide();
				});
		});
	}

	changeControlType(idControlType: number): void {
		if (!this.idParent) {
			this.medicalControlActions = [];
			this.formMedicalControl.get("idAction").setValue(null);
			this.idControlType = idControlType;
			this.hideWorkElementGrid(this.idControlType);
			this.medicalControlActionService.get(idControlType).subscribe(
				(data: Select[]) => {
					this.medicalControlActions = this.fromAbsenceRequested
						? data.filter((x) => this.enumAbsence.some((item) => item === x.id))
						: data;
					if (!this.fromAbsenceRequested) {
						const isWorkAccidentControlType = idControlType === EnumControlType.WorkAccident ? true : false;
						this.medicalControlService.workAccidentControlType$.next(isWorkAccidentControlType);
					}
					if (idControlType === EnumControlType.WorkAccident) {
						this.action = EnumMedicalControlActions.absence;
					}
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			);
		}
	}

	viewInputsFromAbsenceRequest(): boolean {
		return this.idParent ? this.viewInputsFromAbsenceRequestTracking() : this.viewInputsFromAbsenceRequestMedicalControl();
	}

	viewInputsFromAbsenceRequestTracking(): boolean {
		return this.isAbsence && this.viewToggleAbsenceRequested && !this.editing ? true : false;
	}

	viewInputsFromAbsenceRequestMedicalControl(): boolean {
		return this.isAbsence && !this.editing ? true : false;
	}

	getEmployeeData(idPerson: number, fileNumber: boolean): void {
		this.formMedicalControl.get("idEmployee").setValue(idPerson);

		this.idPerson = idPerson;
		this.medicalControlService.medicalControlData.idEmployee = idPerson;
		this.personService.getPersonById(idPerson).subscribe(
			(person: PersonalInformation) => {
				if (person) {
					this.personalInformation = new PersonalInformation();
					this.personalInformation.id = idPerson;
					this.personalInformation.fileNumber = this.fileNumber;
					this.personalInformation.name = person.name;
					this.personalInformation.lastName = person.lastName;

					this.setEmployeeFullNames(this.personalInformation, fileNumber);
				}
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	setEmployeeFullNames(data: any, fileNumber: boolean): void {
		this.fullName = fileNumber ? `${data.fileNumber} - ${data.lastName}, ${data.name}` : `${data.lastName}, ${data.name}`;
	}

	getPerson(idPerson: number): Promise<any> {
		return new Promise((resolve, reject) => {
			this.personService.getEmployeeById(idPerson).subscribe(
				(data: any) => {
					this.fileNumber = data.fileNumber;
					return resolve("ok");
				},
				() => {
					return reject();
				}
			);
		});
	}

	getMedicalControl(id: number): void {
		this.getMedicalControlData(id).then((data) => {
			this.spinner.show();
			this.action = data.idAction ? data.idAction : this.getActionFromTracking(data.idTrackingType);
			this.medicalControlService.idControlTypeParent = data.idControlType;
			this.setMedicalControlData(data).then(() => {
				this.editing = true;
				this.showEmployeeSearch = false;
				this.doctor = this.medicalControlService.medicalControlData.idOccupationalDoctor !== null ? true : false;

				this.changeControlType(this.medicalControlService.medicalControlData.idControlType);
				this.setDoctorValidators();
				this.idPerson = this.medicalControlService.medicalControlData.idEmployee;

				this.getPerson(this.medicalControlService.medicalControlData.idEmployee)
					.then(() => {
						this.getEmployeeData(this.medicalControlService.medicalControlData.idEmployee, true);
						if (this.medicalControlService.medicalControlData.absence.id) {
							this.isAbsence = true;
							this.getAbsenceById(data.idAbsence).then((absenceData) => {
								const newMedicalControlFromAbsence = false;
								this.setAbsenceData(absenceData, newMedicalControlFromAbsence);
							});
						} else {
							this.isAbsence = false;
							this.setMedicalForm();
						}
						this.spinner.hide();
					})
					.catch((error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						}
						this.spinner.hide();
					});
			});
		});
	}

	getActionFromTracking(id: number): number {
		const absenceAction = 4;
		return this.trackingTypes?.find((x) => id === x.id).createAbsence ? absenceAction : 0;
	}

	setMedicalControlData(data: any): Promise<any> {
		return new Promise((resolve) => {
			this.medicalControlService.medicalControlData = {
				...data,
				absence: {
					id: data.idAbsence,
					idAbsenceType: 0,
					idPathology: 0,
					dateFrom: "",
					dateTo: "",
					restDays: 0
				},
				idParent: data.idParent,
				idTrackingType: data.idTrackingType
			};
			if (data.idAbsence) {
				const modify = true;
				this.pathologies = this.idParent === 0 ? this.setControlPathologies(data, modify) : [];
			}

			return resolve("ok");
		});
	}

	setMedicalForm(): void {
		this.formMedicalControl.reset({
			...this.medicalControlService.medicalControlData,
			timeControl: moment(this.medicalControlService.medicalControlData.date).format("HH:mm"),
			doctor: this.doctor,
			employee: this.fullName,
			idAbsenceType: this.medicalControlService.medicalControlData.absence.idAbsenceType,
			enrollment: this.medicalControlService.medicalControlData.privateDoctorEnrollment,
			idAbsence: this.medicalControlService.medicalControlData.absence.id,
			idFile: this.formMedicalControl.get("idFile").value
		});
		this.setFileData();

		this.setEnrollment(this.medicalControlService.medicalControlData.idOccupationalDoctor);
		this.selectDateValid();
		if (!this.isAbsence && this.action !== EnumMedicalControlActions.examination && this.action > 0) {
			this.medicalControlService.setForm(this.medicalControlService.medicalControlData);
		}
		this.getTracking();
	}

	setFileData(): void {
		const fileId = this.medicalControlService.medicalControlData.idFile;
		const acceptParams: UploadFileParameter = {
			entityTypeId: this.entityTypeIdEmployee,
			entityId: this.idPerson ? this.idPerson : this.medicalControlService.medicalControlData.idEmployee,
			fileTypeId: this.idFileType,
			active: this.active,
			fileType: this.fileType,
			fileToUpload: null
		};
		this.dragDropFileService
			.getfileById(fileId, acceptParams)
			.then((res) => {
				const description = res.description;
				const iconClass = `file-${this.dragDropFileService.getExtensionFile(res.url).replace(".", "")}`;
				const fileData = {
					file: {
						file: res,
						urlName: this.dragDropFileService.getFileName(res.url),
						iconClass: iconClass,
						url: res.url,
						idFile: res.fileId,
						description: description
					}
				};
				this.updateData = fileData;
			})
			.catch((error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			});
	}

	setAbsence(): void {
		if (this.medicalControlService.medicalControlData.absence.id) {
			this.medicalControlService.setAbsenceForm(this.medicalControlService.medicalControlData);
		}
	}

	getAbsenceById(id: number): Promise<AbsenceRequest> {
		return new Promise((resolve, reject) => {
			this.spinner.show();
			this.absenceService
				.getById(id)
				.subscribe(
					(data: AbsenceRequest) => {
						resolve(data);
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						}
						reject();
					}
				)
				.add(() => {
					this.spinner.hide();
				});
		});
	}

	setAbsenceData(data: AbsenceRequest, newMedicalControlFromAbsence: boolean): void {
		this.getAbsenceType(data, newMedicalControlFromAbsence);
		this.medicalControlService.medicalControlData.absence.idAbsenceType = data.idAbsenceType;
		this.medicalControlService.medicalControlData.idEmployee = Number(data.employee.id);
		this.medicalControlService.medicalControlData.absence.id = data.id;
		this.medicalControlService.medicalControlData.absence.dateTo = data.dateTo;
		this.medicalControlService.medicalControlData.absence.dateFrom = data.dateFrom;
		this.medicalControlService.medicalControlData.absence.idAbsenceType = data.idAbsenceType;
		this.medicalControlService.medicalControlData.absence.idPathology = data.idPathology;
		this.medicalControlService.medicalControlData.absence.restDays = this.getDifferenceInDays(
			new Date(data.dateFrom),
			new Date(data.dateTo)
		);
		this.idCertificate = data.idCertificate;
		this.setAbsence();
		if (newMedicalControlFromAbsence) {
			this.medicalControlService.medicalControlData.id = 0;
			this.idPerson = Number(data.employee.id);
			this.medicalControlService.medicalControlData.absence.id = data.id;
			this.fileNumber = data.employee.fileNumber;
			this.getEmployeeData(this.idPerson, true);
			this.medicalControlService.medicalControlData.diagnosis = data.description;
		}
		if (data.idPathology) {
			const push = true;
			this.setMedicalControlPathologies(data.idPathology, push);
		}
	}

	setMedicalControlPathologies(id: number, push: boolean): void {
		const method = push ? "push" : "unshift";
		this.getPathologyById(id).then((pathology: Pathologies) => {
			pathology.isModify = false;
			pathology.allowSaveInMedicalControl = true;
			this.medicalControlService.medicalControlData.pathologies = new Array<Pathologies>();
			this.medicalControlService.medicalControlData.pathologies[method](pathology);
			this.pathologies = new Array<Pathologies>();
			this.pathologies[method](pathology);
		});
	}

	getAbsenceType(data: AbsenceRequest, newMedicalControlFromAbsence: boolean): void {
		this.absenceTypeService.getById(data.idAbsenceType).subscribe(
			(type: AbsenceType) => {
				this.medicalControlService.medicalControlData.absence.idBehaviour = type.idBehaviour;
				this.getAvailableAbsences([
					{
						id: data.id,
						startDate: data.dateFrom,
						endDate: data.dateTo,
						type: type,
						idStatus: data.status.id,
						employee: null,
						costOfCenter: null,
						processed: null,
						duration: null,
						fromRhpro: null,
						idPathology: data.idPathology,
						description: data.description
					}
				]);
				if (newMedicalControlFromAbsence) {
					this.medicalControlTypes = this.filterMedicalControlTypes(type.idBehaviour);
				}
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	changeFromAbsenceRequested(): void {
		this.isAbsence = !this.isAbsence;
		if (this.isAbsence) {
			this.getAbsences();
		} else {
			this.medicalControlService.actionForm.get("id").setValue(null);
			this.deletePathology = this.medicalControlService.medicalControlData.absence.idPathology;
			this.medicalControlService.medicalControlData.absence.id = 0;
			this.clearActionAbsenceForm();
			this.absenceSearch = undefined;
		}
	}

	clearActionAbsenceForm(): void {
		const data = new MedicalControl();
		data.id = 0;
		const absence = new MedicalControlAbsence();
		absence.idCountry = this.country.id;
		this.medicalControlService.setAbsenceForm(data);
	}

	getAbsences(): void {
		this.spinner.show();
		this.absencesTypeList = [];
		this.formMedicalControl.get("idAbsenceType").setValue(null);
		this.absencesTypeRangeList = [];
		this.formMedicalControl.get("idAbsence").setValue(null);
		const date =
			this.viewToggleAbsenceRequested && this.idParent
				? moment(new Date(this.formMedicalControl.get("dateTo").value))
						.add(1, "days")
						.utc(true)
				: moment(new Date().getUTCFullYear()).add(-365, "days");
		const absenceFilter = new AbsenceFilterParameters();
		absenceFilter.fileNumber = this.fileNumber;
		absenceFilter.absencePeriodRange = new DateRange();
		absenceFilter.absencePeriodRange.start = date.toString();
		const isSetMedicalControlAbsenceType =
			this.formMedicalControl.get("idTrackingType").value === EnumTrackingType.absenceExtension ||
			this.formMedicalControl.get("idTrackingType").value === EnumTrackingType.reopening
				? true
				: false;
		if (this.viewToggleAbsenceRequested && isSetMedicalControlAbsenceType) {
			this.medicalControlService.filterAbsenceTypeWhenAbsenceExtension$.subscribe((data) => {
				absenceFilter.idAbsenceType = data ? data : 0;
			});
		}
		this.searchEngineFilterService
			.getAbsencesByFilter(absenceFilter)
			.subscribe(
				(data: any) => {
					this.getSearchMedicalControls().then((medicalControls) => {
						const filterData = data.paginationList
							.filter((x) => medicalControls.indexOf(x.id) === -1)
							.filter((elem1, pos, arr) => arr.findIndex((elem2) => elem2.id === elem1.id) === pos);
						this.absenceSearch = filterData.map((x) => ({
							...x,
							idStatus: x.status.idStatus,
							description: x.description,
							type: {
								id: x.type.idType,
								name: x.type.name,
								active: x.type.active,
								occupationalHealth: x.type.occupationalhealth,
								idBehaviour: x.type.idBehaviour
							}
						}));
						this.getAvailableAbsences(this.absenceSearch);
					});
				},
				(error) => {
					if (error.Status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	getAvailableAbsences(data: FilterAbsence[]): void {
		let healthAbsences = data.filter((item) => {
			return (
				item.type.occupationalHealth === true &&
				(item.idStatus === AbsenceStatusEnum.approved || item.idStatus === AbsenceStatusEnum.rectify)
			);
		});
		if (
			this.idParent &&
			(this.formMedicalControl.get("idTrackingType").value === EnumTrackingType.absenceExtension ||
				this.formMedicalControl.get("idTrackingType").value === EnumTrackingType.reopening)
		) {
			healthAbsences = this.filterTrackingTypeExtensionAbsence(healthAbsences);
		}
		const absenceTypes = healthAbsences.map((x) => {
			return {
				id: x.type.id,
				description: x.type.name
			};
		});
		this.absencesTypeList = absenceTypes.reduce((types, pos) => {
			if (!types.some((item) => item.id === pos.id && item.description === pos.description)) {
				types.push(pos);
			}
			return types;
		}, []);
		this.setAbsenceRange(healthAbsences)
			.then(() => {
				if (this.medicalControlService.medicalControlData.absence.id) {
					this.setMedicalForm();
					this.absencesTypeRangeList = this.absencesRangeList;
				}
			})
			.catch((error) => {
				this.alertService.error(error, "medicalControlError");
			});
	}

	filterTrackingTypeExtensionAbsence(healthAbsences: FilterAbsence[]): FilterAbsence[] {
		return healthAbsences.filter((x) => x.type.id === this.medicalControlData.absence.idAbsenceType);
	}

	setAbsenceRange(healthAbsences: FilterAbsence[]): Promise<any> {
		return new Promise((resolve, reject) => {
			if (healthAbsences.length > 0) {
				this.absencesRangeList = healthAbsences
					.map((x) => {
						const startDate = moment(x.startDate);
						const endDate = moment(x.endDate);
						return {
							id: x.id,
							idType: x.type.id,
							startDate: x.startDate,
							idBehaviour: x.type.idBehaviour,
							description: `${startDate.format(this.userPreference.dateFormat.format.toUpperCase())} - ${endDate.format(
								this.userPreference.dateFormat.format.toUpperCase()
							)}`
						};
					})
					.reverse();
			} else {
				const errorMessage = new Array<string>();
				errorMessage.push("theEmployeeHasNotAbsences");
				return reject(errorMessage);
			}
			return resolve("ok");
		});
	}

	absenceTypeChange(id: number): void {
		this.formMedicalControl.get("idAbsence").setValue("");
		this.absencesTypeRangeList = this.absencesRangeList.filter((item) => {
			return item.idType === id;
		});
	}

	getSearchMedicalControls(): Promise<number[]> {
		return new Promise((resolve, reject) => {
			const date = moment(new Date().getUTCFullYear()).add(-365, "days");
			const medicalControlFilter = new MedicalControlsFilterParameters();
			medicalControlFilter.fileNumber = this.fileNumber;
			medicalControlFilter.medicalControlRange = new DateRange();
			medicalControlFilter.medicalControlRange.start = date.toString();
			this.searchEngineFilterService.getMedicalControlByFilter(medicalControlFilter).subscribe(
				(data: MedicalControlList) => {
					const idMedicalControls = data.paginationList;
					return resolve(idMedicalControls.map((x) => x.idAbsence));
				},
				(error) => {
					if (error.Status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
					return reject();
				}
			);
		});
	}

	hideWorkElementGrid(idControlType: EnumControlType): void {
		this.hiddenWorkElementGrid = !(idControlType === EnumControlType.WorkAccident);
	}

	changeAction(idAction: number): void {
		this.action = idAction;
		if (this.idControlType !== EnumControlType.AccidentComplaint && this.generateAbsence(idAction)) {
			this.medicalControlService.onLoadActionAbsence.subscribe(() => {
				this.setDateRangeValidators();
			});
		} else {
			this.unsetDateRangeValidators();
		}
		if (this.viewPathologies && !this.fromAbsenceRequested) {
			const actionGenerateAbsence = this.enumAbsence.find((x) => x === this.action);
			this.viewPathologies = actionGenerateAbsence ? true : false;
			this.pathologies = [];
		}
		if (this.idParent === 0 && !this.fromAbsenceRequested) {
			const isWorkAccidentControlType = this.formMedicalControl.get("idControlType").value === EnumControlType.WorkAccident ? true : false;
			this.medicalControlService.workAccidentControlType$.next(isWorkAccidentControlType);
		}
	}

	generateAbsence(idAction: number): boolean {
		return (
			idAction === EnumMedicalControlActions.professionalIllness ||
			idAction === EnumMedicalControlActions.inItinere ||
			idAction === EnumMedicalControlActions.workAccident ||
			idAction === EnumMedicalControlActions.absence
		);
	}

	changePrivateDoctor(doctor: boolean): void {
		this.doctor = doctor;
		this.setDoctorValidators();
		this.formMedicalControl.get("enrollment").setValue("");
		if (this.doctor) {
			this.formMedicalControl.get("privateDoctorName").setValue("");
		} else {
			this.formMedicalControl.get("idOccupationalDoctor").setValue(0);
		}
	}

	setDoctorValidators(): void {
		if (this.doctor) {
			this.formMedicalControl.controls["enrollment"].disable();
			this.formMedicalControl.get("idOccupationalDoctor").setValidators(Validators.required);
			this.formMedicalControl.get("privateDoctorName").setValidators(null);
		} else {
			this.formMedicalControl.get("idOccupationalDoctor").setValidators(null);
			this.formMedicalControl
				.get("privateDoctorName")
				.setValidators([Validators.required, Validators.minLength(0), Validators.maxLength(50)]);
			this.formMedicalControl.controls["enrollment"].enable();
		}
	}

	setEnrollment(id: number): void {
		if (id) {
			this.formMedicalControl.get("enrollment").setValue(null);
			const doctor = this.doctors.find((x) => x.id === id);
			this.formMedicalControl.get("enrollment").setValue(doctor.enrollment);
		}
	}

	getDifferenceInDays(dateFrom: Date, dateTo: Date): number {
		const diff = Math.abs(dateTo.getTime() - dateFrom.getTime());
		return Math.ceil(diff / (1000 * 3600 * 24)) + 1;
	}

	getEmployee(event: any, fileNumber: boolean = false): void {
		const employee: FilterEmployee = event;
		this.idPerson = employee.idPerson;
		this.fileNumber = employee.fileNumber;
		this.formMedicalControl.get("idEmployee").setValue(employee.idPerson);

		this.getEmployeeData(employee.idPerson, fileNumber);
		if (this.fromAbsenceRequested) {
			this.getAbsences();
			this.isAbsence = true;
		}
		if (this.enumAbsence.find((x) => x === this.action)) {
			this.getAbsenceTypes();
		}
	}

	getAbsenceTypes(): void {
		if (this.medicalControlService.actionForm.get("dateFrom").value) {
			this.medicalControlService.getAbsenceTypes$.next(this.idPerson);
		}
	}

	selectAbsence(id: number): void {
		const data = this.absenceSearch.find((x) => x.id === id);
		if (!this.idParent) {
			this.pathologies = [];
			this.medicalControlTypes = this.filterMedicalControlTypes(data.type.idBehaviour);
		}
		this.medicalControlService.medicalControlData.absence.id = id;
		this.medicalControlService.medicalControlData.absence.idAbsenceType = data.type.id;
		this.medicalControlService.medicalControlData.absence.dateFrom = data.startDate;
		this.medicalControlService.medicalControlData.absence.idPathology = data.idPathology;
		this.medicalControlService.medicalControlData.diagnosis = data.description;
		this.formMedicalControl.get("diagnosis").setValue(data.description);

		this.medicalControlService.medicalControlData.absence.restDays = this.getDifferenceInDays(
			new Date(data.startDate),
			new Date(data.endDate)
		);
		if (data.idPathology) {
			const push = this.pathologies.length > 0 ? true : false;
			this.setMedicalControlPathologies(data.idPathology, push);
		}
		this.medicalControlService.setAbsenceForm(this.medicalControlService.medicalControlData);
		this.selectDateValid();
	}

	getPathologyById(id: number): Promise<any> {
		return new Promise((resolve, reject) => {
			this.spinner.show();
			let pathology = new Pathologies();
			this.patholgyService
				.getById(id)
				.subscribe(
					(data: Pathologies) => {
						pathology = data;
					},
					(error) => {
						if (error.Status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						}
						reject(error);
					}
				)
				.add(() => {
					resolve(pathology);
					this.spinner.hide();
				});
		});
	}

	goToBack(): void {
		this.router.navigateByUrl(this.getUrlForRoute());
	}

	setIdCertificate(): void {
		this.formMedicalControl.get("idFile").setValue(this.dragAndDropFile.fileId);
		this.formMedicalControl.markAsDirty();
	}

	selectDateValid(): void {
		if (this.isAbsence) {
			const absenceSelected = this.absencesRangeList.find((x) => x.id === this.formMedicalControl.get("idAbsence").value);
			if (absenceSelected) {
				this.minSelectDate = absenceSelected.startDate;
			}
		}
	}

	setPathology(value: Array<Pathologies>): void {
		this.pathologies = value;
		this.pathologiesChange = true;
	}

	setDateTime(): Date {
		let date = new Date(this.formMedicalControl.get("date").value);
		date = new Date(date.getTime() + date.getTimezoneOffset() * 60000);
		const time = this.formMedicalControl.get("timeControl").value;
		const hour = Number(moment(time, ["HH.mm a"]).format("HH"));
		const minutes = Number(moment(time, ["HH.mm a"]).format("mm"));
		const seconds = 0;
		date.setHours(hour, minutes, seconds, 0);
		return date;
	}

	formatDateAndTimeToISO(value: Date): string {
		const milliseconds = 60000;
		const dateTime = new Date(value);
		return new Date(new Date(dateTime.getTime() - dateTime.getTimezoneOffset() * milliseconds)).toISOString();
	}

	errorFile(message: string[]): void {
		this.alertService.error(message, "medicalControlError");
	}

	submit(): void {
		if (!this.notValid()) {
			const actionAbsence = this.enumAbsence.some((item) => item === this.action);

			if (!this.medicalControlService.medicalControlData.absence.id && actionAbsence) {
				this.newAbsence();
			} else {
				const existsFile = this.formMedicalControl.get("filename").value ? true : false;
				if (existsFile) {
					this.dragAndDropFile.save().then(() => {
						this.addMedicalControl(false, this.medicalControlService.medicalControlData.absence.id);
					});
				} else {
					this.addMedicalControl(false, this.medicalControlService.medicalControlData.absence.id);
				}
			}
		} else {
			this.formMedicalControl.markAllAsTouched();
		}
	}

	filterPathologies(): Array<number> {
		let pathologies = new Array<Pathologies>();
		pathologies = this.pathologies.filter((x) => x.allowSaveInMedicalControl);
		return pathologies.map((element) => element.id);
	}

	addMedicalControl(isAbsence: boolean, absenceId: number = null): void {
		this.spinner.show();
		const method = this.formMedicalControl.get("id").value !== 0 ? "put" : "post";
		const enrollment = this.doctor === false ? this.formMedicalControl.get("enrollment").value : null;
		const action = this.formMedicalControl.get("idTrackingType").value ? null : this.formMedicalControl.get("idAction").value;
		const date = this.setDateTime();
		const medicalControl = {
			...this.formMedicalControl.value,
			idEmployee: this.idPerson,
			idAbsence: absenceId ? absenceId : null,
			date: this.formatDateAndTimeToISO(date),
			privateDoctorEnrollment: enrollment,
			breakTime: this.medicalControlService.medicalControlData.breakTime,
			testDate: this.medicalControlService.medicalControlData.testDate,
			testResult: this.medicalControlService.medicalControlData.testResult,
			idAction: action,
			pathologies: this.filterPathologies(),
			idFile: this.dragAndDropFile.fileId
		};
		if (method === "put") {
			this.setFileData();
		}

		this.medicalControlService[method](medicalControl)
			.subscribe(
				() => {
					const fileNumber = false;
					this.setEmployeeFullNames(this.personalInformation, fileNumber);
					if (this.formMedicalControl.get("id").value) {
						this.showSnackbar("modifiedSuccessfully");
						this.router.navigateByUrl(this.getUrlForRoute());
					} else {
						this.feedbackOk();
					}
				},
				(error) => {
					if (isAbsence) {
						this.deleteAbsence()
							.then(() => {
								this.feedbackError();
							})
							.catch((errorAbsence) => {
								this.spinner.hide();
								this.alertService.error(errorAbsence.error.Error, "medicalControlError");
							});
					} else if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	isReopening(): boolean {
		return this.formMedicalControl.get("idTrackingType").value === EnumTrackingType.reopening || this.isAnAccident() ? true : false;
	}

	isAnAccident(): boolean {
		const reopeningAbsence = this.medicalControlService.absenceTypes.find(
			(x) => x.id === this.medicalControlService.actionForm.get("idAbsenceType").value
		).allowReopening;
		return this.formMedicalControl.get("idTrackingType").value === EnumTrackingType.absenceExtension && reopeningAbsence ? true : false;
	}

	newAbsence(): void {
		const controlDate = new Date(this.formMedicalControl.get("date").value);
		const absenceDate = new Date(this.medicalControlService.actionForm.get("dateFrom").value);
		if (controlDate.toISOString() >= absenceDate.toISOString()) {
			const dateTo = moment(this.medicalControlService.actionForm.get("dateFrom").value).add(
				this.medicalControlService.actionForm.get("restDays").value - 1,
				"days"
			);
			let absence = new Absence();
			const pathology = this.pathologies[0] ? this.pathologies[0].id : null;
			const isReopening = this.isReopening();

			absence = {
				...this.medicalControlService.actionForm.value,
				dateFrom: absenceDate,
				id: isReopening ? this.absenceParentId : 0,
				idEmployee: this.idPerson,
				idPathology: pathology,
				partial: false,
				numberOfHours: 0,
				description: this.formMedicalControl.get("diagnosis").value,
				dateTo: dateTo,
				idCertificate: this.dragAndDropFile.fileId,
				idCountry: this.country.id
			};
			if (isReopening) {
				this.reopening(absence);
			} else {
				this.addAbsence(absence);
			}
		} else {
			this.formMedicalControl.get("date").setErrors({ invalidDateRanges: true });
		}
	}

	addAbsence(absence: any): void {
		this.absenceService.post(absence).subscribe(
			(data) => {
				this.medicalControlService.medicalControlData.absence.id = data;
				const isAbsence = true;
				this.addMedicalControl(isAbsence, this.medicalControlService.medicalControlData.absence.id);
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	reopening(absence: any): void {
		this.absenceService.reopening(absence).subscribe(
			(data: Array<number>) => {
				const absenceAccidentId = data[0];
				this.medicalControlService.medicalControlData.absence.id = data[0];
				const isAbsence = true;
				this.addMedicalControl(isAbsence, absenceAccidentId);
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	deleteAbsence(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.absenceService.delete(this.medicalControlService.medicalControlData.absence.id).subscribe(
				() => {
					resolve("ok");
				},
				(error) => {
					reject(error);
				}
			);
		});
	}

	showSnackbar(message: any): void {
		this.snackBarService.openSnackBar({
			message: message,
			icon: true,
			action: null,
			secondsDuration: 5
		});
	}

	getUrlForRoute(): string {
		let url = this.idParent ? `/healthApp/medical-control/detail/${this.idParent}` : "/healthApp/medical-controls";
		if (this.fromMedicalHistory) {
			url = `/healthApp/medical-history/modify/${this.idPerson}`;
		}
		return url;
	}

	feedbackOk(): void {
		const medicalControl = this.idParent ? "tracking" : "medicalControl";
		const message = `${medicalControl}SaveFeedbackOkMessage`;
		const succesfully = "createdSuccessfully";
		this.successServ.clear();
		const textDescription = `${this.translatePipe.transform(message)} "${this.fullName}" `;
		this.successServ.data.title = succesfully;
		this.successServ.data.imageFeedBack = EnumFeedback.done;
		this.successServ.data.actionContinue = null;
		this.successServ.data.routeContinue = null;
		this.successServ.data.actionFinish = "finish";

		this.successServ.data.routeFinish = this.getUrlForRoute();
		this.successServ.data.description = `${textDescription} `;
		if (this.idParent) {
			this.successServ.data.actionContinue = this.translatePipe.transform("viewDetail");
			this.successServ.data.routeContinue = `/healthApp/medical-control/detail/${this.idParent}`;
		}
		this.router.navigateByUrl("/healthApp/success");
	}

	feedbackError(): void {
		this.successServ.clear();
		const medicalControl = this.idParent ? "tracking" : "medicalControl";
		const warningDescription = this.translatePipe.transform(`${medicalControl}SaveFeedbackErrorMessage`);
		this.successServ.data.title = "warning";
		this.successServ.data.imageFeedBack = EnumFeedback.error;
		this.successServ.data.actionContinue = null;
		this.successServ.data.routeContinue = null;
		this.successServ.data.actionFinish = "finish";
		this.successServ.data.routeFinish = this.getUrlForRoute();
		this.successServ.data.description = `${warningDescription}`;
		this.router.navigateByUrl("/healthApp/success");
	}

	notValid(): boolean {
		const idAction = this.formMedicalControl.get("idAction").value;
		return idAction === EnumMedicalControlActions.examination ||
			idAction === EnumMedicalControlActions.complaintProfessionalIllness ||
			idAction === EnumMedicalControlActions.complaintInItinere ||
			idAction === EnumMedicalControlActions.complaintWorkAccident
			? !this.formMedicalControl.valid && this.formMedicalControl.dirty
			: !(this.formMedicalControl.valid && this.medicalControlService.actionForm?.valid && this.formMedicalControl.dirty);
	}

	setEllipsis(): void {
		this.ellipsisActions.push(
			{
				action: () => {
					this.dragAndDropFile.openViewer();
				},
				icon: IconTypes.view,
				description: "view"
			},
			{
				action: () => {
					this.dragAndDropFile.download();
				},
				icon: IconTypes.download,
				description: "download"
			},
			{
				action: () => {
					this.dragAndDropFile.delete();
				},
				icon: IconTypes.delete,
				description: "delete"
			}
		);
	}

	setFilename(event: Event): void {
		return this.formMedicalControl.get("filename").setValue(event);
	}

	changeDate(date: Date): void {
		this.medicalControlData.date = date;
		this.filterTrackingType();
	}

	clickCancel(): void {
		this.formMedicalControl?.dirty || this.medicalControlService.actionForm?.dirty || this.pathologiesChange
			? this.askForConfirmation()
			: this.goToBack();
	}

	askForConfirmation(): void {
		this.modalService
			.openDialog({
				title: "attention",
				message: "AreYouSureYouWantToGoOut",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.goToBack();
				}
			});
	}
}
