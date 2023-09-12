import { Pipe, PipeTransform, Injectable } from "@angular/core";

import { EnumResources } from "./../../assets/enumResources";
import { MenuService } from "./../services/menu.service";

@Injectable()
@Pipe({
	name: "accessResource"
})
export class AccessResourcePipe implements PipeTransform {
	private _resources = EnumResources;
	constructor(private menuServ: MenuService) {}

	transform(resource: string): any {
		return this.menuServ.validResource(this._resources[resource]);
	}
}
