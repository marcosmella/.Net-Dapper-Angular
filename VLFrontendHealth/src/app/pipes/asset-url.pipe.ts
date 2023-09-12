import { Pipe, PipeTransform, Injectable } from "@angular/core";

import { assetUrl } from "../../single-spa/asset-url";

@Injectable()
@Pipe({
	name: "assetUrl"
})
export class AssetUrlPipe implements PipeTransform {
	public transform(value: any): any {
		return assetUrl(value);
	}
}
