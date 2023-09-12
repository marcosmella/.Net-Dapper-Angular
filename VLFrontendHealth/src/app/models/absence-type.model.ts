import { EnumBehaviour } from "./enum/enum-behaviour.model.ts.enum";
export class AbsenceType {
	id: number;
	name: string;
	description: string;
	active: boolean;
	system: boolean;
	occupationalHealth: boolean;
	allowReopening: boolean;
	idBehaviour: EnumBehaviour;
}
