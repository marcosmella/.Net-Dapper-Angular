import { AbsenceType } from "./absence-type.model";
import { Employee } from "./employee-filter.model";
export class AbsencePagination {
	quantity: number;
	paginationList: Array<FilterAbsence>;
}

export class FilterAbsence {
	id: number;
	startDate: Date;
	endDate: Date;
	idStatus: number;
	type: AbsenceType;
	employee: Employee;
	costOfCenter: string;
	processed: boolean;
	duration: number;
	fromRhpro: boolean;
	idPathology: number;
	description: string;
}

export class Status {
	id: number;
	description: string;
}
