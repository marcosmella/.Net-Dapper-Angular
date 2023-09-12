import { AfterViewInit, Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import * as moment from "moment";

import { Select } from "./../../../models/select.model";
import { UserPreference } from "./../../../models/user-preference.model";
import { EmployeeVaccine, EmployeeVaccines } from "../../../models/employee-vaccine.model";
import { AlertService } from "./../../../services/alert.service";
import { SnackBarService } from "./../../../services/snack-bar.service";
import { EmployeeVaccinesService } from "./../../../services/employee-vaccines.service";
import { VaccinesService } from "./../../../services/vaccines.service";
import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
@Component({
	selector: "app-vaccines",
	templateUrl: "./vaccines.component.html",
	styleUrls: ["./vaccines.component.scss"]
})
export class VaccinesComponent implements OnInit, AfterViewInit {
	@Input() idEmployee: number;
	@Input("enableEditing") set EnableEditing(value: boolean) {
		this.setFormEditingStatus(value);
	}

	@ViewChild("spinnerVaccines", { static: false })
	public spinner: LoadingSpinnerComponent;

	public enableEditing: boolean;
	public vaccines = Array<Select>();
	public employeeVaccines = new EmployeeVaccines();
	vaccine: EmployeeVaccine;

	public form: FormGroup;

	get formVaccines(): FormArray {
		return <FormArray>this.form.get("formVaccines");
	}

	constructor(
		private fb: FormBuilder,
		public userPreference: UserPreference,
		private vaccinesService: VaccinesService,
		private employeeVaccinesService: EmployeeVaccinesService,
		private snackBarService: SnackBarService,
		private alertService: AlertService
	) {
		this.form = this.fb.group({
			formVaccines: fb.array([])
		});
	}

	ngOnInit(): void {}

	setFormEditingStatus(editing: boolean): void {
		this.enableEditing = editing;

		if (editing) {
			this.form.enable();
		} else {
			this.form.disable();
		}
	}

	ngAfterViewInit(): void {
		this.getVaccines();
		this.getEmployeeVaccines(this.idEmployee);
	}

	checkFormEditing(): void {
		if (this.enableEditing) {
			this.formVaccines.enable();
		} else {
			this.formVaccines.disable();
		}
	}

	createItem(element: EmployeeVaccine = null): FormGroup {
		const formItem = this.fb.group({
			idVaccine: [element.idVaccine, [Validators.required]],
			applicationDate: [element.applicationDate, [Validators.required]],
			messageErrors: [[]],
			valid: [element.valid, []]
		});

		return formItem;
	}

	updateItem(index: number): void {
		if (this.formVaccines.controls.length) {
			this.employeeVaccines.vaccines[index] = this.formVaccines.controls[index].value;
		}
		this.employeeVaccines.valid = this.employeeVaccinesValid();
	}

	removeItem(index: number): void {
		this.formVaccines.removeAt(index);
		this.formVaccines.markAsDirty();
		this.employeeVaccines.valid = this.employeeVaccinesValid();
	}

	addItem(): void {
		if (this.employeeVaccinesValid()) {
			const vaccine: EmployeeVaccine = new EmployeeVaccine();
			this.formVaccines.push(this.createItem(vaccine));
			this.formVaccines.markAsDirty();
			this.formVaccines.markAllAsTouched();
		} else {
			this.formVaccines.markAllAsTouched();
		}
		this.updateItem(this.formVaccines.controls.length - 1);
	}

	getVaccines(): void {
		this.spinner.show();
		this.vaccinesService
			.get()
			.subscribe(
				(data) => {
					this.vaccines = data;
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalHealthError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	getEmployeeVaccines(idEmployee: number): void {
		this.spinner.show();
		this.employeeVaccinesService
			.getByEmployee(idEmployee)
			.subscribe(
				(data) => {
					this.employeeVaccines = {
						idEmployee: this.idEmployee,
						vaccines: data,
						valid: this.employeeVaccinesValid()
					};
					this.employeeVaccines.vaccines.forEach((vaccine) => {
						this.createItem(vaccine);
						this.formVaccines.push(this.createItem(vaccine));
						this.formVaccines.markAsDirty();
						this.formVaccines.markAllAsTouched();
					});
					this.form.markAsPristine();
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalHealthError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	employeeVaccinesValid(): boolean {
		let valid = true;
		this.formVaccines.controls.forEach((vaccine) => {
			valid = valid && vaccine.valid;
		});
		return valid;
	}

	save(): void {
		this.spinner.show();
		this.employeeVaccines = {
			idEmployee: this.idEmployee,
			vaccines: this.formVaccines.value,
			valid: this.employeeVaccinesValid()
		};
		this.formVaccines.markAsPristine();
		this.employeeVaccinesService
			.put(this.employeeVaccines)
			.subscribe(
				() => {
					this.snackBarService.openSnackBar({
						message: `modifiedSuccessfully`,
						icon: true,
						secondsDuration: 5,
						action: null
					});
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalHealthError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	clickSave(): void {
		if (!this.employeeVaccinesValid()) {
			this.formVaccines.markAllAsTouched();
			return;
		}
		this.save();
	}

	changeApplicationDate(index: number): void {
		const idVaccine = this.formVaccines.controls[index].value.idVaccine;
		const vaccineDate = moment(this.formVaccines.controls[index].value.applicationDate);
		const applicationDate = vaccineDate.format(this.userPreference.dateFormat.format.toUpperCase());

		this.formVaccines.controls[index].get("messageErrors").setValue([]);
		this.formVaccines.value.forEach((element, i) => {
			const date = moment(element.applicationDate);
			const dateElement = date.format(this.userPreference.dateFormat.format.toUpperCase());

			if (element.idVaccine === idVaccine && dateElement === applicationDate && i !== index) {
				this.formVaccines.controls[index].get("messageErrors").setValue(["applicationVaccineAlreadyExists "]);
				this.formVaccines.controls[index].get("applicationDate").setValue(null);
			}
		});
		this.employeeVaccines.vaccines[index] = this.formVaccines.controls[index].value;

		this.updateItem(index);
	}
}
