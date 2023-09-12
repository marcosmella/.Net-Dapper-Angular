import { AfterViewInit, Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";

import { DisabilityService } from "../../../services/disability.service";
import { Select } from "./../../../models/select.model";
import { AlertService } from "../../../services/alert.service";
import { BloodTypeService } from "./../../../services/blood-type.service";
import { EmployeeMedicalHistoryService } from "../../../services/employee-medical-history.services";
import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
import { EmployeeMedicalHistory } from "../../../models/employee-medical-history.model";
import { EmployeePathologyService } from "./../../../services/employee-pathology.service";
import { SnackBarService } from "../../../services/snack-bar.service";
import { Pathologies, Pathology } from "../../../models/pathology.model";
import { MedicalControlsFilterParameters } from "./../../../models/medical-control.model";
import { MedicalControlsListComponent } from "../../medical-controls-grid/medical-controls-list/medical-controls-list.component";
import { ModalService } from "../../../services/modal.service";

@Component({
	selector: "app-general-data",
	templateUrl: "./general-data.component.html",
	styleUrls: ["./general-data.component.scss"]
})
export class GeneralDataComponent implements OnInit, AfterViewInit {
	@ViewChild("spinnerGeneralData", { static: true }) spinnerGeneralData: LoadingSpinnerComponent;

	@Input("editing") set changeEditorStatus(value: boolean) {
		this.setFormEditingStatus(value);
	}

	@Input("idEmployee") set enableEmployee(value: number) {
		if (value) {
			this.idEmployee = value;
		}
	}

	@Input("fileNumber") set fileNumberEmployee(value: string) {
		if (value) {
			this.showMedicalControl(value);
		}
	}

	@ViewChild("medicalControlList", { static: true })
	public medicalControlList: MedicalControlsListComponent;

	public idEmployee: number;

	public formGeneralData: FormGroup;
	public disabilities: Array<Select>;
	public bloodTypes: Array<Select>;
	public pathologies = new Array<Pathology>();
	public medicalHistory: EmployeeMedicalHistory;
	public personDisability;
	public personPathologies = new Array<Pathology>();
	public personPathologyData = false;
	public editing = false;

	private pathologiesChange = false;

	constructor(
		private modalService: ModalService,
		private fb: FormBuilder,
		private disabilityService: DisabilityService,
		private bloodTypeService: BloodTypeService,
		private alertService: AlertService,
		private employeeMedicalHistoryService: EmployeeMedicalHistoryService,
		private employeePathologyService: EmployeePathologyService,
		private snackBarService: SnackBarService,
		private router: Router
	) {
		this.formGeneralData = this.createItem();
		this.medicalHistory = new EmployeeMedicalHistory();
	}

	ngOnInit(): void {}

	ngAfterViewInit(): void {
		this.setDisability();
		this.setBloodTypes();
		if (this.idEmployee) {
			this.setDataForm();
			this.medicalHistory.id = 0;
		}
	}

	setFormEditingStatus(editing: boolean): void {
		this.editing = editing;

		if (editing) {
			this.formGeneralData.enable();
		} else {
			this.formGeneralData.disable();
		}
	}

	showMedicalControl(fileNumber: string): void {
		const medicalControlsFilter = new MedicalControlsFilterParameters();
		medicalControlsFilter.fileNumber = fileNumber;
		medicalControlsFilter.page = 0;
		medicalControlsFilter.pageSize = 5;
		medicalControlsFilter.orderBy = "fileNumber DESC";

		this.medicalControlList.medicalControlsFilter = medicalControlsFilter;
		this.medicalControlList.getMedicalControlByFilter();
	}

	setDisability(): void {
		this.disabilityService.get().subscribe(
			(data) => {
				this.disabilities = data;
			},
			(error) => {
				this.alertService.error(error.error.Error, "medicalHealthError");
			}
		);
	}

	setBloodTypes(): void {
		this.bloodTypeService.get().subscribe(
			(data) => {
				this.bloodTypes = data;
			},
			(error) => {
				this.alertService.error(error.error.Error, "medicalHealthError");
			}
		);
	}

	createItem(): FormGroup {
		return this.fb.group({
			id: [0],
			idPerson: [null],
			idBloodType: [null, [Validators.required]],
			idBloodPressure: [null, [Validators.required]],
			isRiskGroup: [null, [Validators.required]],
			idPathologies: [null],
			idDisability: [null],
			errorMessage: [[]]
		});
	}

	setDataForm(): void {
		this.spinnerGeneralData.show();
		Promise.all([this.getPersonMedicalHistory(), this.getPersonDisability(), this.getPersonPathologies()])
			.then(() => {
				this.formGeneralData.reset({
					...this.medicalHistory,
					id: this.medicalHistory.id,
					idBloodPressure: this.medicalHistory.idBloodPressure,
					idPerson: this.medicalHistory.idPerson,
					idBloodType: this.medicalHistory.idBloodType,
					isRiskGroup: this.medicalHistory.isRiskGroup,
					idPathologies: this.pathologies,
					idDisability: this.personDisability
				});
			})
			.finally(() => {
				this.spinnerGeneralData.hide();
			});
	}

	getPersonMedicalHistory(): Promise<any> {
		return new Promise((resolve) => {
			this.employeeMedicalHistoryService
				.get(this.idEmployee)
				.subscribe(
					(data: EmployeeMedicalHistory) => {
						this.medicalHistory = data;
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalHealthError");
						}
					}
				)
				.add(() => {
					return resolve("ok");
				});
		});
	}

	getPersonDisability(): Promise<any> {
		return new Promise((resolve) => {
			this.disabilityService
				.getById(this.idEmployee)
				.subscribe(
					(data) => {
						this.personDisability = data.id;
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalHealthError");
						}
					}
				)
				.add(() => {
					return resolve("ok");
				});
		});
	}

	getPersonPathologies(): Promise<any> {
		return new Promise((resolve) => {
			this.employeePathologyService
				.get(this.idEmployee)
				.subscribe(
					(data: any) => {
						this.personPathologyData = true;
						this.pathologies = data.map((x) => ({
							id: x.id,
							description: x.description,
							isModify: true
						}));
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalHealthError");
						}
						this.personPathologyData = false;
					}
				)
				.add(() => {
					return resolve("ok");
				});
		});
	}

	setPathology(value: Array<Pathologies>): void {
		this.pathologies = value;
		this.pathologiesChange = true;
	}

	submit(): void {
		if (this.formGeneralData.valid) {
			this.spinnerGeneralData.show();
			if (this.formGeneralData.value.idDisability !== this.personDisability) {
				this.saveDisability();
			}
			if (this.personPathologyData || this.pathologies.length > 0) {
				this.personPathologyData = true;
				this.savePathologies();
			}
			const method = this.formGeneralData.get("id").value !== 0 ? "put" : "post";
			const message = method === "put" ? "modifiedSuccessfully" : "createdSuccessfully";
			const form = {
				id: this.formGeneralData.get("id").value,
				idPerson: this.idEmployee,
				idBloodType: this.formGeneralData.get("idBloodType").value,
				idBloodPressure: this.formGeneralData.get("idBloodPressure").value,
				isRiskGroup: this.formGeneralData.get("isRiskGroup").value
			};
			this.formGeneralData.markAsPristine();
			this.employeeMedicalHistoryService[method](form)
				.subscribe(
					(data) => {
						this.snackBarService.openSnackBar({
							message: message,
							icon: true,
							secondsDuration: 10,
							action: null
						});
						if (data) {
							this.formGeneralData.get("id").setValue(data);
						}
					},
					(error) => {
						this.alertService.error(error.error.Error, "medicalHealthError");
					}
				)
				.add(() => {
					this.spinnerGeneralData.hide();
				});
		} else {
			this.formGeneralData.markAllAsTouched();
		}
	}

	saveDisability(): Promise<any> {
		let action = "delete";
		const id = this.formGeneralData.value.idDisability ? this.formGeneralData.value.idDisability : undefined;
		if (id) {
			action = this.personDisability !== undefined ? "put" : "post";
		}
		this.personDisability = id;
		return this.disabilityService[action]({ idDisability: id, idPerson: this.idEmployee }).toPromise();
	}

	savePathologies(): Promise<any> {
		const employeePathologies = this.pathologies.map((element) => ({
			id: element.id
		}));
		this.pathologiesChange = false;
		return this.employeePathologyService["put"]({
			idEmployee: this.idEmployee,
			pathologies: employeePathologies
		}).toPromise();
	}

	goToMedicalControl(): void {
		this.router.navigateByUrl(`/healthApp/medical-control?medicalHistoryIdEmployee=${this.idEmployee}`);
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
					this.goToMedicalControl();
				}
			});
	}

	clickAddMedicalControl(): void {
		this.formGeneralData.dirty || this.pathologiesChange ? this.askForConfirmation() : this.goToMedicalControl();
	}
}
