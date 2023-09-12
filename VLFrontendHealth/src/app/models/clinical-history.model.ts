import { Select } from "./select.model";

export class ClinicalHistory {
	id: number;
	idPerson: number;
	idBloodType: Select;
	idBloodPressure: number;
	isRiskGroup: boolean;
	pathologies: Select[];
	idDisability: Select[];
}
