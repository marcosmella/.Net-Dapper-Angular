import { StructureType } from "./structure-type.model";

export class Structure {
	id: number;
	description: string;
	idStructureType?: number;
	externalCode?: string;
	active?: boolean;
	system?: boolean;
	structureType: StructureType;
}
