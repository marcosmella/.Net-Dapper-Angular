import { Pathology } from "./pathology.model";

export class EmployeeMedicalHistory {
	id: number;
	idPerson: number;
	idBloodType: number;
	idBloodPressure: number;
	isRiskGroup: boolean;
	idPathologies: Pathology[];
	idDisability: number;
}

export class EmployeeMedicalHistoryRequest {
	id: number;
	idPerson: number;
	idBloodType: number;
	idBloodPressure: number;
	isRiskGroup: boolean;
}
