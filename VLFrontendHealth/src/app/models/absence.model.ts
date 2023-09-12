import { Status } from "./absence-filter.model";
export class Absence {
	id: number;
	idEmployee: number;
	idAbsenceType: number;
	dateFrom: Date;
	dateTo: Date;
	description: string;
	idCountry: number;
}

export class AbsenceRequest {
	id: number;
	idAbsenceType: number;
	employee: Employeee;
	dateFrom: Date;
	dateTo: Date;
	description: string;
	status: Status;
	idPathology: number;
	partial: boolean;
	numberOfHours: number;
	details: { name: string; dateFrom: Date; dateTo: Date };
	idCertificate: number;
	processed: boolean;
}
export class Employeee {
	id: string;
	fileNumber: string;
	name: string;
	lastName: string;
	secondLastName: string;
	secondName: string;
}
