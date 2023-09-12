export class WorkElementOption {
	id: number;
	description: string;
}
export enum WorkElementFeature {
	color = 1,
	measure = 2,
	weight = 3,
	percentage = 4,
	custom = 5,
	size = 6,
	unity = 7
}

export enum WorkElementCategory {
	accessories = 1,
	tools = 2,
	dress = 3,
	protection = 4
}

export enum WorkElementDeliveryStatus {
	pending = 1,
	sended = 2,
	notSended = 3
}

export class WorkElementDeliveryRow {
	id: number;
	idDelivery: number;
	idElement: number;
	date: Date;
	categoryDescription: string;
	elementDescription: string;
	amount: number;
	status: WorkElementDeliveryStatus;
	option?: WorkElementOption;
	signed: boolean;
	claim: boolean;
	idDetail: number;
	idFile: number;
}

export class WorkElementDelivery {
	id: number;
	reason: string;
	date: Date;
	idFile?: number;
	idTuRecibo?: number;
	signedConfirmationReceived: boolean;
	workElements: WorkElementDeliveryDetail[];
}

export class WorkElementDeliveryDetail {
	idDetail: number;
	idEmployee: number;
	element: WorkElement;
	option?: WorkElementOption;
	amount: number;
	returnDate?: Date;
}

export class WorkElement {
	id: number;
	idCategory: WorkElementCategory;
	description: string;
	externalCode?: string;
	brand?: string;
	claim: boolean;
	accountWithCertification: boolean;
	expirationDate?: Date;
	idFeature?: WorkElementFeature;
	options?: WorkElementOption[];
	signed: boolean;
	returnDate: Date;
	idDetail: number;
}
