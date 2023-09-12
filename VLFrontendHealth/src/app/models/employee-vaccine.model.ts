export class EmployeeVaccine {
	idVaccine: number;
	applicationDate: Date;
	valid = false;
}

export class EmployeeVaccines {
	idEmployee: number;
	vaccines: EmployeeVaccine[] = [];
	valid = false;
}
