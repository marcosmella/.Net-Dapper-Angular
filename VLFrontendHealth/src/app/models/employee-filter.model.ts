export class EmployeePagination {
	quantity: number;
	paginationList: Array<FilterEmployee>;
}

export class Employee {
	idPerson: number;
	fileNumber: string;
	name: string;
	secondName: string;
	lastName: string;
	secondLastName: string;
	document: string;
	active: boolean;
	idDirectReport: number;
}
export class FilterEmployee {
	idPerson: number;
	fileNumber: string;
	name: string;
	secondName: string;
	lastName: string;
	secondLastname: string;
	document: string;
	active: boolean;
	idDirectReport: number;
}

export class DateRange {
	start: string;
	end: string;
}

export class Structures {
	idStructureType: number;
	idStructure: number;
	start: Date;
}

export class FilterParameters {
	lastName: string;
	name: string;
	documentNumber: string;
	fileNumber: string;
	active: boolean;
	hiringDate: DateRange;
	decouplingDate: DateRange;
	structures: Array<Structures>;
	orderBy: Array<string>;
	idBossEmployee: number;
	page: number;
	pageSize: number;
}

export class AbsenceFilterParameters {
	lastName: string;
	name: string;
	documentNumber: string;
	fileNumber: string;
	active: boolean;
	absencePeriodRange: DateRange;
	decouplingDate: DateRange;
	structures: Array<Structures>;
	orderBy: Array<string>;
	idBossEmployee: number;
	idAbsenceType;
	page: number;
	pageSize: number;
}
