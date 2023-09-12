export class EmployeePagination {
	quantity: number;
	paginationList: Array<Employee>;
}

export class Employee {
	idPerson: number;
	fileNumber: string;
	fullName: string;
	name: string;
	secondName: string;
	lastName: string;
	secondLastName: string;
	document: string;
	additionalAttributes: {
		structureTypeDescription_5: string;
		structureTypeDescription_17: string;
		structureTypeDescription_23: string;
	};
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
	structures: Array<Structures>;
	orderBy: Array<string>;
	page: number;
	pageSize: number;
}

export class ClinicalRecordGrid {
	idPerson: number;
	fileNumber: string;
	fullName: string;
	document: string;
	structureTypeDescription5: string;
	structureTypeDescription17: string;
	structureTypeDescription23: string;
}
