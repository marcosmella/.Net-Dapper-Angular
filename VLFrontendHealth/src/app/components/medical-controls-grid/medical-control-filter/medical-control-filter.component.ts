import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Component, EventEmitter, OnInit, Output } from "@angular/core";

import { MedicalControlsFilterParameters } from "./../../../models/medical-control.model";
import { DateValidators } from "./../../../validators/date-validator";
import { MedicalControlTypeService } from "./../../../services/medical-control-type.service";
import { Select } from "./../../../models/select.model";
import { AlertService } from "./../../../services/alert.service";
import { AbsenceTypeService } from "./../../../services/absence-type.service";
import { AbsenceType } from "./../../../models/absence-type.model";
import { AuthService } from "./../../../services/auth.service";
import { MedicalControlActionService } from "./../../../services/medical-control-action.service";
import { StructureTypeService } from "./../../../services/structure-type.service";
import { StructureService } from "./../../../services/structure.service";
import { Structures } from "../../../models/employee-filter.model";
@Component({
	selector: "app-medical-control-filter",
	templateUrl: "./medical-control-filter.component.html",
	styleUrls: ["./medical-control-filter.component.scss"]
})
export class MedicalControlFilterComponent implements OnInit {
	public medicalControlsFilter = new MedicalControlsFilterParameters();
	public formMedicalControlFilter: FormGroup;
	public medicalControlTypes: Array<Select>;
	public absenceTypes: Array<AbsenceType>;
	public medicalControlActions: Array<Select>;
	public structureTypes = new Array<Select>();
	public structures: Array<Select>;

	@Output() onApplyFilter = new EventEmitter();

	constructor(
		private fb: FormBuilder,
		private dateValidators: DateValidators,
		private alertService: AlertService,
		private absenceTypeService: AbsenceTypeService,
		private authService: AuthService,
		private medicalControlTypeService: MedicalControlTypeService,
		private medicalControlActionService: MedicalControlActionService,
		private structureTypeService: StructureTypeService,
		private structureService: StructureService
	) {
		this.createForm();
	}

	ngOnInit(): void {
		const country = this.authService.getTenantCountry().id;
		this.getMedicalControlType();
		this.getMedicalControlActions();
		this.getAbsenceTypes(country);
	}

	createForm(): void {
		this.formMedicalControlFilter = this.fb.group({
			name: [""],
			lastName: [""],
			fileNumber: ["", [Validators.pattern("^[0-9]*$")]],
			idControlType: [null],
			idAbsenceType: [null],
			idMedicalControlAction: [null],
			medicalControlRange: this.fb.group({
				start: ["", [this.dateValidators.dateLessThan("start", "end")]],
				end: ["", [this.dateValidators.dateLessThan("start", "end")]]
			}),
			idStructureType: [""],
			idStructure: [""]
		});
	}

	getAbsenceTypes(idCountry?: number): void {
		this.absenceTypeService.get(idCountry).subscribe(
			(data: AbsenceType[]) => {
				this.absenceTypes = data.filter((x) => x.occupationalHealth === true);
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	getMedicalControlType(): void {
		this.medicalControlTypeService.get().subscribe(
			(data: Select[]) => {
				this.medicalControlTypes = data;
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	getMedicalControlActions(): void {
		this.medicalControlActions = [];
		this.medicalControlActionService.getActions().subscribe(
			(data: Select[]) => {
				this.medicalControlActions = data;
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			}
		);
	}

	clearFilter(): void {
		this.medicalControlsFilter = new MedicalControlsFilterParameters();

		this.updateFilter();
	}

	updateFilter(): void {
		this.formMedicalControlFilter.reset(this.medicalControlsFilter);
	}

	applyFilter(): void {
		const controlType = this.medicalControlTypes.find((x) => x.id === this.formMedicalControlFilter.value.idControlType);
		const absenceType = this.absenceTypes.find((x) => x.id === this.formMedicalControlFilter.value.idAbsenceType);
		const action = this.medicalControlActions?.find((x) => x.id === this.formMedicalControlFilter.value.idAction);
		this.medicalControlsFilter = {
			...this.medicalControlsFilter,
			...this.formMedicalControlFilter.value,
			absenceType: absenceType?.name,
			controlType: controlType?.description,
			action: action?.description,
			structure: this.medicalControlsFilter.structure
		};
		this.onApplyFilter.emit();
	}

	clickApplyFilter(): void {
		this.applyFilter();
	}

	clearProperty(name: string): void {
		if (name === "medicalControlRange") {
			this.formMedicalControlFilter.get(name).get("start").setValue("");
			this.formMedicalControlFilter.get(name).get("end").setValue("");
		}
		if (name === "controlType") {
			this.formMedicalControlFilter.get("idControlType").setValue(null);
			this.getMedicalControlActions();
		}
		if (name === "action") {
			this.formMedicalControlFilter.get("idAction").setValue(null);
		}
		if (name === "absenceType") {
			this.formMedicalControlFilter.get("idAbsenceType").setValue(null);
		}
		if (name === "idStructureType") {
			this.changeStructureType("");
		}
		if (name === "idStructure") {
			this.changeStructure("");
		}
		if (
			name !== "medicalControlRange" &&
			name !== "controlType" &&
			name !== "absenceType" &&
			name !== "idStructureType" &&
			name !== "idStructure"
		) {
			this.formMedicalControlFilter.get(name).setValue(null);
		}

		this.medicalControlsFilter = { ...this.medicalControlsFilter, ...this.formMedicalControlFilter.value };
	}

	filterDisabled(): boolean {
		return !this.formMedicalControlFilter.dirty || !this.formMedicalControlFilter.valid;
	}

	changeControlType(idControlType: number): void {
		this.medicalControlActions = [];

		if (idControlType) {
			this.medicalControlActionService.get(idControlType).subscribe(
				(data: Select[]) => {
					this.medicalControlActions = data;
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			);
		} else {
			this.getMedicalControlActions();
		}
	}

	changeAction(event: any): void {
		this.medicalControlsFilter.actionDescription = event?.description;
	}

	changeStructureType(event: any): void {
		if (!this.formMedicalControlFilter.controls["idStructureType"].value) {
			this.formMedicalControlFilter.controls["idStructure"].setValue("");
			this.medicalControlsFilter.structureTypeDescription = "";
			this.medicalControlsFilter.structure = new Structures();
			return;
		}
		this.medicalControlsFilter.structure = new Structures();
		this.medicalControlsFilter.structure.idStructureType = this.formMedicalControlFilter.controls["idStructureType"].value;
		this.formMedicalControlFilter.controls["idStructure"].setValue(null);
		this.medicalControlsFilter.structureTypeDescription = event.description;
		this.structureService.get(this.medicalControlsFilter.structure.idStructureType).subscribe(
			(data) => {
				this.structures = data;
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "medicalControlError");
				} else {
					this.structures = [];
				}
			}
		);
	}

	changeStructure(event: any): void {
		if (!this.formMedicalControlFilter.controls["idStructure"].value || !event) {
			this.medicalControlsFilter.structure.idStructure = 0;
			this.medicalControlsFilter.structureDescription = "";
			this.formMedicalControlFilter.controls["idStructure"].setValue("");
			return;
		}
		this.medicalControlsFilter.structure.idStructure = this.formMedicalControlFilter.controls["idStructure"].value.id;
		this.medicalControlsFilter.structureDescription = event.description;
	}

	getStructureType(): void {
		if (this.structureTypes.length === 0) {
			this.structureTypeService.get().subscribe(
				(data) => {
					this.structureTypes = data;
				},
				(error) => {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			);
		}
	}
}
