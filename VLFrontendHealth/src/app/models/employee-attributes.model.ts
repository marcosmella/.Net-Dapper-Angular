import { Structure } from "./structure.model";

export class EmployeeAttributes {
	id: number;
	idEmployee: number;
	idReasonType: number;
	reason: string;
	startDate: Date;
	endDate: Date;
	structure: Structure;
}
