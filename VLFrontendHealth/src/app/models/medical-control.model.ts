import { Structures } from "./clinical-record-filter.model";
import { Pathologies } from "./pathology.model";
import { AbsenceType } from "./absence-type.model";
import { MedicalControlAbsence } from "./medical-control-absence.model";

export class MedicalControl {
	id: number;
	idEmployee: number;
	date: Date;
	idControlType: number;
	idAction: number;
	idMedicalService: number;
	idOccupationalDoctor?: number;
	privateDoctorName: string;
	privateDoctorEnrollment: string;
	diagnosis: string;
	absence: MedicalControlAbsence;
	idFile: number;
	idFileComplaint: number;
	breakTime: number;
	testDate: Date;
	testResult: boolean;
	pathologies: Array<Pathologies>;
	tracking: Array<MedicalControlChild>;
	_type: "MedicalControl";
}

export class MedicalControlChild {
	id: number;
	idEmployee: number;
	date: Date;
	idControlType: number;
	idAction: number;
	idMedicalService: number;
	idOccupationalDoctor?: number;
	privateDoctorName: string;
	privateDoctorEnrollment: string;
	diagnosis: string;
	idAbsence: number;
	idFile: number;
	breakTime: number;
	testDate: Date;
	testResult: boolean;
	idParent: number;
	idTrackingType: number;
	pathologies: Array<Pathologies>;
	_type: "MedicalControlChild";
}

export class MedicalControlRow {
	idMedicalControl: number;
	idPerson: number;
	controlDate: Date;
	fileNumber: string;
	fullName: string;
	name: string;
	lastName: string;
	controlType: MedicalControlType;
	action: MedicalControlAction;
	absenceDateStart: string;
	absenceDateEnd: string;
	absenceType: AbsenceType;
	idAbsence: number;
	range: string;
	breakTime: number;
	absenceTypeDescription: string;
	duration: string;
	idPathology: number;
	pathology: string;
}
export class MedicalControlList {
	quantity: number;
	paginationList: Array<MedicalControlRow>;
}

export class MedicalControlExcel {
	idPerson: number;
	fileNumber: string;
}

export class MedicalControlsFilterParameters {
	idEmployee: number;
	lastName: string;
	name: string;
	fileNumber: string;
	idControlType: number;
	descriptionControlType: string;
	idAbsenceType: number;
	descriptionAbsenceType: number;
	medicalControlRange: DateRange;
	structure: Structures;
	idMedicalControlAction: number;
	actionDescription: string;
	idStructure: number;
	idStructureType: number;
	structureTypeDescription: string;
	structureDescription: string;
	page = 0;
	pageSize = 25;
	orderBy = "fileNumber DESC";
}

export class DateRange {
	start: string = null;
	end: string = null;
}

export class MedicalControlAction {
	idAction: number;
	actionDescription: string;
}

export class MedicalControlType {
	idControlType: number;
	controlTypeDescription: string;
}

export class MedicalControlTrackingDate {
	absenceDateFrom: Date;
	absenceDateTo: Date;
	lastMedicalControlDate: Date;
	lastMedicalControlTrackingType: number;
}

export class MedicalControlGrid {
	id: number;
	controlActionTypeDescription: string;
	controlDate: Date;
	controlRestDays: number;
	controlTypeDescription: string;
	idAbsence: number;
	tracking: Array<MedicalControlTrackingGrid>;
}

export class MedicalControlTrackingGrid {
	id: number;
	trackingDate: Date;
	trackingDescription: string;
	trackingControlDescription: string;
	trackingRestDays: number;
	idAbsence: number;
	viewOptions: boolean;
}
