import { Injectable } from "@angular/core";

import { EnumTrackingType } from "../../models/tracking-type.enum";
import { MedicalControl } from "../../models/medical-control.model";
import { EnumMedicalControlActions } from "../../models/medicalControlAction.enum";
import { EnumControlType } from "../../models/enum/control-type.model";

@Injectable({
	providedIn: "root"
})
export class TdTrackingService {
	private medicalControl: MedicalControl;

	private trackingTypeConditions: Function[] = [];
	private trackingTypeActions: EnumTrackingType[][] = [[]];

	constructor() {
		this.trackingTypeInicializeDecisionTable();
	}
	//#region Inicialize Rules
	trackingTypeInicializeRule1(): void {
		this.trackingTypeConditions[1] = () => {
			return (
				!this.isMedicalControlOfControlType(EnumControlType.WorkAccident) &&
				!this.isMedicalControlOfControlType(EnumControlType.AccidentComplaint) &&
				!this.isMedicalControlOfActionType(EnumMedicalControlActions.absence)
			);
		};
		this.trackingTypeActions[1] = [];
	}
	trackingTypeInicializeRule2(): void {
		this.trackingTypeConditions[2] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.WorkAccident) &&
				this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[2] = [
			EnumTrackingType.revision,
			EnumTrackingType.absenceExtension,
			EnumTrackingType.patientReleaseDate,
			EnumTrackingType.absenceNotEndorsed
		];
	}
	trackingTypeInicializeRule3(): void {
		this.trackingTypeConditions[3] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.WorkAccident) &&
				this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[3] = [
			EnumTrackingType.revision,
			EnumTrackingType.absenceExtension,
			EnumTrackingType.patientReleaseDate,
			EnumTrackingType.absenceNotEndorsed
		];
	}
	trackingTypeInicializeRule4(): void {
		this.trackingTypeConditions[4] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.WorkAccident) &&
				this.existAbsence() &&
				this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[4] = [EnumTrackingType.revision, EnumTrackingType.reopening, EnumTrackingType.absenceNotEndorsed];
	}
	trackingTypeInicializeRule5(): void {
		this.trackingTypeConditions[5] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.AccidentComplaint) &&
				!this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.rejection)
			);
		};
		this.trackingTypeActions[5] = [EnumTrackingType.revision, EnumTrackingType.rejection, EnumTrackingType.generationAbsence];
	}
	trackingTypeInicializeRule6(): void {
		this.trackingTypeConditions[6] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.AccidentComplaint) &&
				this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfType(EnumTrackingType.rejection) &&
				this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[6] = [
			EnumTrackingType.revision,
			EnumTrackingType.absenceExtension,
			EnumTrackingType.patientReleaseDate,
			EnumTrackingType.absenceNotEndorsed
		];
	}
	trackingTypeInicializeRule7(): void {
		this.trackingTypeConditions[7] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.AccidentComplaint) &&
				this.existAbsence() &&
				this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfType(EnumTrackingType.rejection) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[7] = [EnumTrackingType.revision, EnumTrackingType.reopening];
	}
	trackingTypeInicializeRule8(): void {
		this.trackingTypeConditions[8] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.AccidentComplaint) &&
				!this.existAbsence() &&
				this.lastTrackingOfType(EnumTrackingType.rejection)
			);
		};
		this.trackingTypeActions[8] = [EnumTrackingType.revision];
	}
	trackingTypeInicializeRule9(): void {
		this.trackingTypeConditions[9] = () => {
			return (
				this.isMedicalControlOfActionType(EnumMedicalControlActions.absence) &&
				this.existAbsence() &&
				this.lastTrackingOfType(EnumTrackingType.patientReleaseDate)
			);
		};
		this.trackingTypeActions[9] = [EnumTrackingType.revision];
	}
	trackingTypeInicializeRule10(): void {
		this.trackingTypeConditions[10] = () => {
			return (
				this.isMedicalControlOfActionType(EnumMedicalControlActions.absence) &&
				this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[10] = [EnumTrackingType.revision, EnumTrackingType.absenceExtension, EnumTrackingType.patientReleaseDate];
	}
	trackingTypeInicializeRule11(): void {
		this.trackingTypeConditions[11] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.AccidentComplaint) &&
				this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[11] = [EnumTrackingType.revision, EnumTrackingType.absenceExtension, EnumTrackingType.patientReleaseDate];
	}
	trackingTypeInicializeRule12(): void {
		this.trackingTypeConditions[12] = () => {
			return (
				this.isMedicalControlOfControlType(EnumControlType.WorkAccident) &&
				!this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[12] = [];
	}
	trackingTypeInicializeRule13(): void {
		this.trackingTypeConditions[13] = () => {
			return (
				this.isMedicalControlOfActionType(EnumMedicalControlActions.absence) &&
				!this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[13] = [];
	}

	trackingTypeInicializeRule14(): void {
		this.trackingTypeConditions[14] = () => {
			return (
				this.isMedicalControlOfActionType(EnumMedicalControlActions.absence) &&
				this.existAbsence() &&
				!this.lastTrackingOfType(EnumTrackingType.patientReleaseDate) &&
				!this.lastTrackingOfAbsence()
			);
		};
		this.trackingTypeActions[14] = [EnumTrackingType.revision, EnumTrackingType.absenceExtension, EnumTrackingType.patientReleaseDate];
	}
	//#endregion

	trackingTypeInicializeDecisionTable(): void {
		this.trackingTypeInicializeRule1();
		this.trackingTypeInicializeRule2();
		this.trackingTypeInicializeRule3();
		this.trackingTypeInicializeRule4();
		this.trackingTypeInicializeRule5();
		this.trackingTypeInicializeRule6();
		this.trackingTypeInicializeRule7();
		this.trackingTypeInicializeRule8();
		this.trackingTypeInicializeRule9();
		this.trackingTypeInicializeRule10();
		this.trackingTypeInicializeRule11();
		this.trackingTypeInicializeRule12();
		this.trackingTypeInicializeRule13();
		this.trackingTypeInicializeRule14();
	}

	trackingTypeEvaluateRule(ruleNumber: number): EnumTrackingType[] {
		if (this.trackingTypeConditions[ruleNumber]()) {
			return this.trackingTypeActions[ruleNumber];
		}
	}

	enableTrackingTypes(tracking: MedicalControl): EnumTrackingType[] {
		this.medicalControl = tracking;

		for (let ruleNumber = 1; ruleNumber < this.trackingTypeConditions.length; ruleNumber++) {
			const ruleActions = this.trackingTypeEvaluateRule(ruleNumber);

			if (ruleActions) {
				return ruleActions;
			}
		}
	}

	//#region private
	private isMedicalControlOfControlType(type: EnumControlType): boolean {
		return this.medicalControl.idControlType === type;
	}

	private isMedicalControlOfActionType(type: EnumMedicalControlActions): boolean {
		return this.medicalControl.idAction === type;
	}

	private existAbsence(): boolean {
		let existAbsence = this.medicalControl.absence?.id !== null;
		if (this.medicalControl.tracking.length !== 0) {
			existAbsence = existAbsence || this.medicalControl.tracking.find((x) => x.idAbsence !== null) !== undefined ? true : false;
		}
		return existAbsence;
	}

	private lastTrackingOfType(type: EnumTrackingType): boolean {
		return this.medicalControl.tracking.length !== 0
			? this.medicalControl.tracking[this.medicalControl.tracking.length - 1].idTrackingType === type
			: false;
	}

	private lastTrackingOfAbsence(): boolean {
		if (this.medicalControl.tracking.length === 0) {
			return this.medicalControl.absence.id !== undefined;
		}
		return this.medicalControl.tracking[this.medicalControl.tracking.length - 1].idAbsence !== null ? true : false;
	}
	//#endregion
}
